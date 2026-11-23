using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LootPulse;

public sealed class Config
{
    public string RoomCode { get; set; } = "";
    public string Profile { get; set; } = "";
    public string? ReplayFolder { get; set; }
    public bool OverlayEnabled { get; set; } = true;
    public bool OverlayLocked { get; set; } = false;
    public bool OverlayOnlyInGame { get; set; } = false;
    public int OverlayScale { get; set; } = 100;
    public int OverlayOpacity { get; set; } = 70;
    public int OverlayTextOpacity { get; set; } = 100;
    public int OverlayX { get; set; } = 24;
    public int OverlayY { get; set; } = 140;
    public int ManualChests { get; set; } = 0;
    public string UpdateRepo { get; set; } = "fjdjdhidjfj/LootPulse";
    public bool AutoCheckUpdates { get; set; } = true;
}

public sealed class ChestEvent
{
    public float TimeSeconds { get; set; }
    public bool IsDestroyed { get; set; }
    public string Label { get; set; } = "Chest";
}

public sealed class MatchSummary
{
    public string SessionId { get; set; } = "";
    public string ReplayFile { get; set; } = "";
    public DateTime DateUtc { get; set; } = DateTime.UtcNow;
    public int DurationSeconds { get; set; }
    public int ChestsOpened { get; set; }
    public int ChestsDestroyed { get; set; }
    public int SquadChests { get; set; }
    public List<string> SquadRoster { get; set; } = new();
    public List<ChestEvent> Events { get; set; } = new();
    public double ChestsPerMinute => DurationSeconds > 0 ? Math.Round((double)ChestsOpened / (DurationSeconds / 60.0), 2) : 0;
}

public sealed class TrackerData
{
    public int LifetimeChests { get; set; }
    public int LifetimeDestroyed { get; set; }
    public Dictionary<string, bool> ProcessedFiles { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public List<MatchSummary> Matches { get; set; } = new();
}

public sealed class TeammatePeer
{
    public string ClientId { get; set; } = "";
    public string PlayerName { get; set; } = "";
    public string EpicId { get; set; } = "";
    public string RoomCode { get; set; } = "";
    public int MatchChests { get; set; }
    public int LastMatchChests { get; set; }
    public int SessionChests { get; set; }
    public int TotalChests { get; set; }
    public int MatchSeconds { get; set; }
    public DateTime LastSeenUtc { get; set; } = DateTime.UtcNow;

    public int AgeSeconds => (int)Math.Max(0, (DateTime.UtcNow - LastSeenUtc).TotalSeconds);
    public string Health => AgeSeconds switch
    {
        < 6 => "good",
        < 18 => "fair",
        _ => "reconnecting"
    };

    public bool IsOnline => (DateTime.UtcNow - LastSeenUtc).TotalSeconds < 60;
}

public sealed class PartyPacket
{
    public string ClientId { get; set; } = "";
    public string Type { get; set; } = "pulse";
    public string RoomCode { get; set; } = "";
    public string PlayerName { get; set; } = "";
    public string EpicId { get; set; } = "";
    public int MatchChests { get; set; }
    public int LastMatchChests { get; set; }
    public int SessionChests { get; set; }
    public int TotalChests { get; set; }
    public int MatchSeconds { get; set; }
    public DateTime SentUtc { get; set; } = DateTime.UtcNow;
}

public sealed class Storage
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public string Folder { get; }
    public string ConfigFile => Path.Combine(Folder, "config.json");
    public string DataFile => Path.Combine(Folder, "data.json");

    public Storage(string folder)
    {
        Folder = folder;
        Directory.CreateDirectory(Folder);
    }

    public Config LoadConfig()
    {
        if (File.Exists(ConfigFile))
        {
            try
            {
                var cfg = JsonSerializer.Deserialize<Config>(File.ReadAllText(ConfigFile), JsonOpts);
                if (cfg != null) return cfg;
            }
            catch { /* fallback to default */ }
        }
        var created = new Config();
        SaveConfig(created);
        return created;
    }

    public void SaveConfig(Config config) => AtomicSave(ConfigFile, config);

    public TrackerData LoadData()
    {
        if (File.Exists(DataFile))
        {
            try
            {
                var d = JsonSerializer.Deserialize<TrackerData>(File.ReadAllText(DataFile), JsonOpts);
                if (d != null) return d;
            }
            catch { /* fallback to new */ }
        }
        return new TrackerData();
    }

    public void SaveData(TrackerData data) => AtomicSave(DataFile, data);

    public string ExportBackupJson()
    {
        var backup = new
        {
            Version = "2.0",
            ExportedUtc = DateTime.UtcNow,
            Config = LoadConfig(),
            Data = LoadData()
        };
        return JsonSerializer.Serialize(backup, JsonOpts);
    }

    public bool ImportBackupJson(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("Config", out var cfgElem))
            {
                var cfg = JsonSerializer.Deserialize<Config>(cfgElem.GetRawText(), JsonOpts);
                if (cfg != null) SaveConfig(cfg);
            }
            if (doc.RootElement.TryGetProperty("Data", out var dataElem))
            {
                var data = JsonSerializer.Deserialize<TrackerData>(dataElem.GetRawText(), JsonOpts);
                if (data != null) SaveData(data);
            }
            return true;
        }
        catch { return false; }
    }

    private static void AtomicSave<T>(string path, T obj)
    {
        var tmp = path + ".tmp";
        File.WriteAllText(tmp, JsonSerializer.Serialize(obj, JsonOpts));
        File.Move(tmp, path, overwrite: true);
    }
}
