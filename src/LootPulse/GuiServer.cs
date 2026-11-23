using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LootPulse;

public sealed class GuiServer
{
    private readonly HttpListener _listener = new();
    private readonly string _token;
    private readonly MatchEngine _engine;
    private readonly PartyHub _partyHub;
    private readonly Storage _storage;
    private readonly Config _config;
    private readonly Action<string> _onWindowControl;
    private UpdateCheckResult? _latestUpdate;

    public string Url { get; }

    public void SetAvailableUpdate(UpdateCheckResult update) => _latestUpdate = update;

    public GuiServer(MatchEngine engine, PartyHub partyHub, Storage storage, Config config, Action<string> onWindowControl)
    {
        _engine = engine;
        _partyHub = partyHub;
        _storage = storage;
        _config = config;
        _onWindowControl = onWindowControl;

        _token = Convert.ToHexString(RandomNumberGenerator.GetBytes(16)).ToLowerInvariant();

        // Bind to a free localhost port
        var tcp = new System.Net.Sockets.TcpListener(IPAddress.Loopback, 0);
        tcp.Start();
        var port = ((IPEndPoint)tcp.LocalEndpoint).Port;
        tcp.Stop();

        Url = $"http://127.0.0.1:{port}/?token={_token}";
        _listener.Prefixes.Add($"http://127.0.0.1:{port}/");
    }

    public void Start()
    {
        _listener.Start();
        Task.Run(RunLoop);
    }

    public void Stop()
    {
        try { _listener.Stop(); } catch { /* ignore */ }
    }

    private async Task RunLoop()
    {
        while (_listener.IsListening)
        {
            try
            {
                var ctx = await _listener.GetContextAsync();
                _ = Task.Run(() => HandleRequest(ctx));
            }
            catch { break; }
        }
    }

    private async Task HandleRequest(HttpListenerContext ctx)
    {
        var req = ctx.Request;
        var res = ctx.Response;

        var token = req.QueryString["token"];
        if (token != _token && req.Url?.AbsolutePath != "/favicon.ico")
        {
            res.StatusCode = 403;
            res.Close();
            return;
        }

        try
        {
            var path = req.Url?.AbsolutePath ?? "/";
            if (path == "/" || path == "/index.html")
            {
                var html = GuiPage.Render(_token);
                SendResponse(res, "text/html; charset=utf-8", Encoding.UTF8.GetBytes(html));
            }
            else if (path == "/state")
            {
                var peers = _partyHub.GetConnectedPeers().Select(p => new
                {
                    name = p.PlayerName,
                    matchChests = p.MatchChests,
                    lastChests = p.LastMatchChests > 0 ? p.LastMatchChests : p.MatchChests,
                    allChests = p.TotalChests,
                    sessionChests = p.SessionChests
                }).ToList();

                var matches = _engine.Matches.Select(m => new
                {
                    sessionId = m.SessionId,
                    date = m.DateUtc.ToLocalTime().ToString("dd.MM HH:mm"),
                    dateIso = m.DateUtc.ToLocalTime().ToString("yyyy-MM-dd"),
                    timestamp = new DateTimeOffset(m.DateUtc).ToUnixTimeMilliseconds(),
                    duration = m.DurationSeconds,
                    chests = m.ChestsOpened + m.ChestsDestroyed,
                    squadChests = m.SquadChests,
                    squad = m.SquadRoster
                }).ToList();

                var state = new
                {
                    totalChests = _engine.LifetimeChests,
                    sessionChests = _engine.SessionChests,
                    lastMatchChests = _engine.LastMatchChests,
                    currentMatchChests = _engine.CurrentMatchChests,
                    matchSeconds = _engine.CurrentMatchSeconds,
                    roomCode = _config.RoomCode,
                    profile = _config.Profile,
                    demosFolder = _config.ReplayFolder,
                    overlayOpacity = _config.OverlayOpacity,
                    overlayTextOpacity = _config.OverlayTextOpacity,
                    overlayOnlyInGame = _config.OverlayOnlyInGame,
                    currentVersion = AppInfo.Version,
                    updateRepo = _config.UpdateRepo,
                    autoCheckUpdates = _config.AutoCheckUpdates,
                    availableUpdate = _latestUpdate != null && _latestUpdate.HasUpdate ? new
                    {
                        version = _latestUpdate.LatestVersion,
                        downloadUrl = _latestUpdate.DownloadUrl,
                        sizeBytes = _latestUpdate.FileSizeBytes,
                        changelog = _latestUpdate.Changelog
                    } : null,
                    peers,
                    matches
                };

                var json = JsonSerializer.Serialize(state);
                SendResponse(res, "application/json", Encoding.UTF8.GetBytes(json));
            }
            else if (path == "/action")
            {
                var act = req.QueryString["act"] ?? "";
                var val = req.QueryString["val"] ?? "";

                if (act == "checkUpdate")
                {
                    var updateResult = await Updater.CheckAsync(_config.UpdateRepo, AppInfo.Version);
                    if (updateResult.HasUpdate) _latestUpdate = updateResult;
                    var resJson = JsonSerializer.Serialize(updateResult, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    SendResponse(res, "application/json", Encoding.UTF8.GetBytes(resJson));
                    return;
                }
                if (act == "applyUpdate")
                {
                    var dUrl = !string.IsNullOrEmpty(val) ? val : _latestUpdate?.DownloadUrl;
                    if (!string.IsNullOrEmpty(dUrl))
                    {
                        _ = Task.Run(async () =>
                        {
                            try { await Updater.ApplyAsync(dUrl); } catch { /* best effort */ }
                        });
                        SendResponse(res, "application/json", Encoding.UTF8.GetBytes("{\"ok\":true}"));
                        return;
                    }
                    SendResponse(res, "application/json", Encoding.UTF8.GetBytes("{\"ok\":false,\"error\":\"Ссылка отсутствует\"}"));
                    return;
                }

                HandleAction(act, val);
                SendResponse(res, "application/json", Encoding.UTF8.GetBytes("{\"ok\":true}"));
            }
            else
            {
                res.StatusCode = 404;
                res.Close();
            }
        }
        catch
        {
            res.StatusCode = 500;
            res.Close();
        }
    }

    private void HandleAction(string act, string val)
    {
        switch (act)
        {
            case "setRoom":
                _config.RoomCode = val.Trim();
                _storage.SaveConfig(_config);
                _partyHub.UpdateConfig(_config);
                _partyHub.BroadcastPulse(_engine.CurrentMatchChests, _engine.SessionChests, _engine.LifetimeChests, _engine.LastMatchChests, _engine.CurrentMatchSeconds);
                break;

            case "setProfile":
                _config.Profile = val.Trim();
                _storage.SaveConfig(_config);
                break;

            case "addManual":
                if (int.TryParse(val, out var add))
                {
                    _config.ManualChests += add;
                    _storage.SaveConfig(_config);
                }
                break;

            case "toggleOverlay":
                NativeOverlay.ToggleVisibility();
                _storage.SaveConfig(_config);
                break;

            case "toggleLock":
                NativeOverlay.ToggleLock();
                _storage.SaveConfig(_config);
                break;

            case "setOverlayOpacity":
                if (int.TryParse(val, out var op))
                {
                    _config.OverlayOpacity = op;
                    _storage.SaveConfig(_config);
                    NativeOverlay.SetOpacity(op);
                }
                break;

            case "setOverlayTextOpacity":
                if (int.TryParse(val, out var top))
                {
                    _config.OverlayTextOpacity = top;
                    _storage.SaveConfig(_config);
                    NativeOverlay.SetTextOpacity(top);
                }
                break;

            case "setOverlayOnlyInGame":
                _config.OverlayOnlyInGame = val == "true";
                _storage.SaveConfig(_config);
                NativeOverlay.SetOnlyInGame(_config.OverlayOnlyInGame);
                break;

            case "setUpdateRepo":
                _config.UpdateRepo = val.Trim();
                _storage.SaveConfig(_config);
                break;

            case "toggleAutoCheckUpdates":
                _config.AutoCheckUpdates = val == "true";
                _storage.SaveConfig(_config);
                break;

            case "wdrag":
            case "wmin":
            case "wmax":
            case "wclose":
                _onWindowControl?.Invoke(act);
                break;
        }
    }

    private static void SendResponse(HttpListenerResponse res, string contentType, byte[] data)
    {
        res.ContentType = contentType;
        res.ContentLength64 = data.Length;
        res.OutputStream.Write(data, 0, data.Length);
        res.Close();
    }
}
