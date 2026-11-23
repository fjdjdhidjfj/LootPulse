using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LootPulse;

public sealed class MatchEngine
{
    private static readonly TimeSpan QuietPeriod = TimeSpan.FromSeconds(5);
    private readonly Storage _storage;
    private readonly PartyHub _partyHub;
    private Config _config;
    private TrackerData _data;

    private FileSystemWatcher? _watcher;
    private readonly Dictionary<string, long> _fileSizes = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, DateTime> _fileLastTouched = new(StringComparer.OrdinalIgnoreCase);
    private string? _activeReplayPath;
    private DateTime? _matchStartUtc;
    private long _lastPeekedLength = 0;
    private DateTime _lastPeekUtc = DateTime.MinValue;

    private CancellationTokenSource? _engineCts;

    public string MatchStatus { get; private set; } = "InLobby"; // InLobby, InMatch, Analyzing
    public int CurrentMatchSeconds => _matchStartUtc.HasValue ? (int)Math.Max(0, (DateTime.UtcNow - _matchStartUtc.Value).TotalSeconds) : 0;
    public int SessionChests { get; private set; }
    public int CurrentMatchChests { get; private set; }
    public MatchSummary? LastMatch { get; private set; }

    public int LifetimeChests => _data.LifetimeChests + _data.LifetimeDestroyed + _config.ManualChests;
    public int LastMatchChests => (LastMatch?.ChestsOpened ?? 0) + (LastMatch?.ChestsDestroyed ?? 0);
    public IReadOnlyList<MatchSummary> Matches => _data.Matches;

    public Action? OnStateChanged { get; set; }

    public MatchEngine(Storage storage, Config config, TrackerData data, PartyHub partyHub)
    {
        _storage = storage;
        _config = config;
        _data = data;
        _partyHub = partyHub;
        LastMatch = _data.Matches.LastOrDefault();
    }

    public void UpdateConfig(Config config)
    {
        var folderChanged = !string.Equals(_config.ReplayFolder, config.ReplayFolder, StringComparison.OrdinalIgnoreCase);
        _config = config;
        _partyHub.UpdateConfig(config);
        if (folderChanged) StartWatcher();
    }

    public void Start()
    {
        _engineCts = new CancellationTokenSource();
        var ct = _engineCts.Token;

        StartWatcher();
        _partyHub.Start();

        // Background loop for match timer, quiet period check, and heartbeat
        Task.Run(async () =>
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    CheckReplayActivity();
                    _partyHub.BroadcastPulse(CurrentMatchChests, SessionChests, LifetimeChests, LastMatchChests, CurrentMatchSeconds);
                    OnStateChanged?.Invoke();
                }
                catch { /* loop guard */ }

                await Task.Delay(2000, ct).ConfigureAwait(false);
            }
        }, ct);
    }

    public void Stop()
    {
        _engineCts?.Cancel();
        _watcher?.Dispose();
        _watcher = null;
        _partyHub.Stop();
    }

    private string ResolveDemosFolder()
    {
        if (!string.IsNullOrWhiteSpace(_config.ReplayFolder) && Directory.Exists(_config.ReplayFolder))
        {
            return _config.ReplayFolder;
        }

        var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var demos = Path.Combine(local, "FortniteGame", "Saved", "Demos");
        return Directory.Exists(demos) ? demos : "";
    }

    private void StartWatcher()
    {
        _watcher?.Dispose();
        _watcher = null;

        var folder = ResolveDemosFolder();
        if (string.IsNullOrEmpty(folder)) return;

        try
        {
            _watcher = new FileSystemWatcher(folder, "*.replay")
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size,
                EnableRaisingEvents = true
            };

            _watcher.Changed += (_, e) => OnReplayTouched(e.FullPath);
            _watcher.Created += (_, e) => OnReplayTouched(e.FullPath);
        }
        catch { /* watcher initialization error */ }
    }

    private void OnReplayTouched(string path)
    {
        if (_data.ProcessedFiles.ContainsKey(Path.GetFileName(path))) return;

        lock (_fileLastTouched)
        {
            _fileLastTouched[path] = DateTime.UtcNow;
            _activeReplayPath = path;

            if (MatchStatus == "InLobby")
            {
                MatchStatus = "InMatch";
                _matchStartUtc = DateTime.UtcNow;
                CurrentMatchChests = 0;
            }
        }
    }

    private void CheckReplayActivity()
    {
        if (string.IsNullOrEmpty(_activeReplayPath) || MatchStatus != "InMatch") return;

        DateTime lastTouch;
        lock (_fileLastTouched)
        {
            if (!_fileLastTouched.TryGetValue(_activeReplayPath, out lastTouch)) return;
        }

        // Live peek replay while match is writing chunks
        TryLivePeekReplay();

        // If untouched for QuietPeriod (match concluded and key written by Fortnite)
        if (DateTime.UtcNow - lastTouch >= QuietPeriod)
        {
            MatchStatus = "Analyzing";
            OnStateChanged?.Invoke();

            var replayToParse = _activeReplayPath;
            _activeReplayPath = null;
            _matchStartUtc = null;
            _lastPeekedLength = 0;

            Task.Run(() => ParseFinishedReplay(replayToParse));
        }
    }

    private void TryLivePeekReplay()
    {
        if (string.IsNullOrEmpty(_activeReplayPath) || DateTime.UtcNow - _lastPeekUtc < TimeSpan.FromSeconds(3.5)) return;

        try
        {
            var fi = new FileInfo(_activeReplayPath);
            if (!fi.Exists || fi.Length <= _lastPeekedLength || fi.Length < 64 * 1024) return;

            _lastPeekUtc = DateTime.UtcNow;
            _lastPeekedLength = fi.Length;

            using var stream = new FileStream(_activeReplayPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var analyzer = new ChestAnalyzer();
            var summary = analyzer.Analyze(stream, fi.Name, _config.Profile);

            int totalChests = summary.ChestsOpened + summary.ChestsDestroyed;
            if (totalChests != CurrentMatchChests)
            {
                CurrentMatchChests = totalChests;
                _partyHub.BroadcastPulse(CurrentMatchChests, SessionChests + CurrentMatchChests, LifetimeChests + CurrentMatchChests, totalChests, CurrentMatchSeconds);
                OnStateChanged?.Invoke();
            }
        }
        catch { /* live stream read chunk guard */ }
    }

    private void ParseFinishedReplay(string path)
    {
        var fileName = Path.GetFileName(path);
        try
        {
            if (!File.Exists(path)) return;

            // Wait brief moment for file handle release
            Thread.Sleep(1000);

            using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var analyzer = new ChestAnalyzer();
            var summary = analyzer.Analyze(stream, fileName, _config.Profile);

            int totalEarned = summary.ChestsOpened + summary.ChestsDestroyed;

            // Fetch current connected squad total
            summary.SquadChests = _partyHub.GetSquadMatchChests() + totalEarned;
            summary.SquadRoster = _partyHub.GetConnectedPeers().Select(p => p.PlayerName).ToList();

            // Update stats
            _data.LifetimeChests += totalEarned;
            _data.ProcessedFiles[fileName] = true;
            _data.Matches.Add(summary);

            SessionChests += totalEarned;
            LastMatch = summary;

            _storage.SaveData(_data);
        }
        catch { /* error analyzing replay */ }
        finally
        {
            MatchStatus = "InLobby";
            CurrentMatchChests = 0;
            _partyHub.BroadcastPulse(0, SessionChests, LifetimeChests, LastMatchChests, 0);
            OnStateChanged?.Invoke();
        }
    }
}
