using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;

namespace LootPulse.Tests;

public class StorageTests
{
    [Fact]
    public void Storage_SavesAndLoadsConfigCorrectly()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "LootPulse_Test_" + Guid.NewGuid().ToString("N"));
        try
        {
            var storage = new Storage(tempDir);
            var cfg = new Config
            {
                RoomCode = "squad_test_99",
                Profile = "ProPlayer",
                ManualChests = 42,
                OverlayEnabled = true,
                OverlayScale = 110
            };

            storage.SaveConfig(cfg);
            var loaded = storage.LoadConfig();

            Assert.Equal("squad_test_99", loaded.RoomCode);
            Assert.Equal("ProPlayer", loaded.Profile);
            Assert.Equal(42, loaded.ManualChests);
            Assert.Equal(110, loaded.OverlayScale);
        }
        finally
        {
            if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void Storage_SavesAndLoadsTrackerDataCorrectly()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "LootPulse_Test_" + Guid.NewGuid().ToString("N"));
        try
        {
            var storage = new Storage(tempDir);
            var data = new TrackerData
            {
                LifetimeChests = 550,
                LifetimeDestroyed = 35
            };
            data.ProcessedFiles["match1.replay"] = true;
            data.Matches.Add(new MatchSummary
            {
                SessionId = "sess_001",
                ChestsOpened = 12,
                ChestsDestroyed = 2,
                SquadChests = 30
            });

            storage.SaveData(data);
            var loaded = storage.LoadData();

            Assert.Equal(550, loaded.LifetimeChests);
            Assert.Equal(35, loaded.LifetimeDestroyed);
            Assert.True(loaded.ProcessedFiles.ContainsKey("match1.replay"));
            Assert.Single(loaded.Matches);
            Assert.Equal(12, loaded.Matches[0].ChestsOpened);
            Assert.Equal(30, loaded.Matches[0].SquadChests);
        }
        finally
        {
            if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void Storage_ExportsAndImportsBackupCorrectly()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "LootPulse_Backup_" + Guid.NewGuid().ToString("N"));
        try
        {
            var storage = new Storage(tempDir);
            var cfg = new Config { RoomCode = "chug_room_42", Profile = "ApexLoot" };
            var data = new TrackerData { LifetimeChests = 120, LifetimeDestroyed = 10 };
            storage.SaveConfig(cfg);
            storage.SaveData(data);

            var backupJson = storage.ExportBackupJson();
            Assert.Contains("chug_room_42", backupJson);
            Assert.Contains("ApexLoot", backupJson);

            // Import into a fresh folder
            var targetDir = Path.Combine(Path.GetTempPath(), "LootPulse_Restore_" + Guid.NewGuid().ToString("N"));
            try
            {
                var targetStorage = new Storage(targetDir);
                var success = targetStorage.ImportBackupJson(backupJson);
                Assert.True(success);

                var restoredCfg = targetStorage.LoadConfig();
                var restoredData = targetStorage.LoadData();

                Assert.Equal("chug_room_42", restoredCfg.RoomCode);
                Assert.Equal("ApexLoot", restoredCfg.Profile);
                Assert.Equal(120, restoredData.LifetimeChests);
                Assert.Equal(10, restoredData.LifetimeDestroyed);
            }
            finally
            {
                if (Directory.Exists(targetDir)) Directory.Delete(targetDir, true);
            }
        }
        finally
        {
            if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void MatchSummary_CalculatesChestsPerMinute_AndRecordsEvents()
    {
        var match = new MatchSummary
        {
            DurationSeconds = 600, // 10 minutes
            ChestsOpened = 15,
            ChestsDestroyed = 3
        };
        match.Events.Add(new ChestEvent { TimeSeconds = 45f, IsDestroyed = false, Label = "Chest" });
        match.Events.Add(new ChestEvent { TimeSeconds = 120f, IsDestroyed = false, Label = "Chest" });
        match.Events.Add(new ChestEvent { TimeSeconds = 300f, IsDestroyed = true, Label = "Chest" });

        // 15 chests / (600 / 60) min = 1.5 CPM
        Assert.Equal(1.5, match.ChestsPerMinute);
        Assert.Equal(3, match.Events.Count);
        Assert.False(match.Events[0].IsDestroyed);
        Assert.True(match.Events[2].IsDestroyed);
    }
}

public class PartyHubTests
{
    [Fact]
    public void PartyHub_StrictRoomIsolation_IgnoresDifferentRoom()
    {
        var config = new Config { RoomCode = "secret_room_1", Profile = "PlayerA" };
        var hub = new PartyHub(config);

        // Packet from another room
        var wrongPacket = new PartyPacket
        {
            RoomCode = "different_room_2",
            PlayerName = "TeammateB",
            MatchChests = 15,
            SentUtc = DateTime.UtcNow
        };

        // Use reflection to call private ProcessIncomingJson
        var method = typeof(PartyHub).GetMethod("ProcessIncomingJson", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(method);
        method.Invoke(hub, new object[] { JsonSerializer.Serialize(wrongPacket) });

        // Must be empty
        Assert.Empty(hub.GetConnectedPeers());
        Assert.Equal(0, hub.GetSquadMatchChests());
    }

    [Fact]
    public void PartyHub_RejectsSelfEcho()
    {
        var config = new Config { RoomCode = "my_squad", Profile = "LocalHostPlayer" };
        var hub = new PartyHub(config);

        var selfPacket = new PartyPacket
        {
            ClientId = PartyHub.LocalClientId,
            RoomCode = "my_squad",
            PlayerName = "LocalHostPlayer",
            MatchChests = 25,
            SentUtc = DateTime.UtcNow
        };

        var method = typeof(PartyHub).GetMethod("ProcessIncomingJson", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(method);
        method.Invoke(hub, new object[] { JsonSerializer.Serialize(selfPacket) });

        Assert.Empty(hub.GetConnectedPeers());
        Assert.Equal(0, hub.GetSquadMatchChests());
    }

    [Fact]
    public void PartyHub_AcceptsMatchingRoomPeers_AndAggregatesSquadChests()
    {
        var config = new Config { RoomCode = "victory_clan", Profile = "Me" };
        var hub = new PartyHub(config);

        var mate1 = new PartyPacket
        {
            ClientId = "peer_remote_1",
            RoomCode = "victory_clan",
            PlayerName = "Friend1",
            MatchChests = 8,
            LastMatchChests = 8,
            SessionChests = 24,
            SentUtc = DateTime.UtcNow
        };
        var mate2 = new PartyPacket
        {
            ClientId = "peer_remote_2",
            RoomCode = "victory_clan",
            PlayerName = "Friend2",
            MatchChests = 11,
            LastMatchChests = 11,
            SessionChests = 30,
            SentUtc = DateTime.UtcNow
        };

        var method = typeof(PartyHub).GetMethod("ProcessIncomingJson", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(method);
        method.Invoke(hub, new object[] { JsonSerializer.Serialize(mate1) });
        method.Invoke(hub, new object[] { JsonSerializer.Serialize(mate2) });

        var peers = hub.GetConnectedPeers();
        Assert.Equal(2, peers.Count);
        Assert.Equal(19, hub.GetSquadMatchChests()); // 8 + 11
        Assert.Equal(54, hub.GetSquadSessionChests()); // 24 + 30
    }

    [Fact]
    public void PartyHub_DeduplicatesSamePlayerOnReconnect()
    {
        var config = new Config { RoomCode = "victory_clan", Profile = "Me" };
        var hub = new PartyHub(config);

        var firstSession = new PartyPacket
        {
            ClientId = "client_sess_1",
            RoomCode = "victory_clan",
            PlayerName = "ketamori",
            MatchChests = 8,
            SessionChests = 24,
            SentUtc = DateTime.UtcNow
        };

        // Teammate restarts LootPulse and gets a new ClientId
        var reconnectedSession = new PartyPacket
        {
            ClientId = "client_sess_2",
            RoomCode = "victory_clan",
            PlayerName = "ketamori",
            MatchChests = 14,
            SessionChests = 30,
            SentUtc = DateTime.UtcNow
        };

        var method = typeof(PartyHub).GetMethod("ProcessIncomingJson", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(method);
        method.Invoke(hub, new object[] { JsonSerializer.Serialize(firstSession) });
        method.Invoke(hub, new object[] { JsonSerializer.Serialize(reconnectedSession) });

        var peers = hub.GetConnectedPeers();
        Assert.Single(peers);
        Assert.Equal("ketamori", peers[0].PlayerName);
        Assert.Equal(14, peers[0].MatchChests);
        Assert.Equal(30, peers[0].SessionChests);
    }

    [Fact]
    public void TeammatePeer_CalculatesHealthAndAgeCorrectly()
    {
        var peerRecent = new TeammatePeer { LastSeenUtc = DateTime.UtcNow.AddSeconds(-2) };
        Assert.Equal("good", peerRecent.Health);
        Assert.True(peerRecent.AgeSeconds >= 1 && peerRecent.AgeSeconds <= 3);

        var peerFair = new TeammatePeer { LastSeenUtc = DateTime.UtcNow.AddSeconds(-10) };
        Assert.Equal("fair", peerFair.Health);

        var peerLag = new TeammatePeer { LastSeenUtc = DateTime.UtcNow.AddSeconds(-25) };
        Assert.Equal("reconnecting", peerLag.Health);
    }

    [Fact]
    public void Config_SavesOpacityAndScaleSettingsCorrectly()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "LootPulse_Cfg_" + Guid.NewGuid().ToString("N"));
        try
        {
            var storage = new Storage(tempDir);
            var cfg = new Config
            {
                OverlayOpacity = 75,
                OverlayTextOpacity = 90,
                OverlayScale = 120,
                OverlayOnlyInGame = true,
                RoomCode = "room_abc"
            };
            storage.SaveConfig(cfg);

            var loaded = storage.LoadConfig();
            Assert.Equal(75, loaded.OverlayOpacity);
            Assert.Equal(90, loaded.OverlayTextOpacity);
            Assert.Equal(120, loaded.OverlayScale);
            Assert.True(loaded.OverlayOnlyInGame);
            Assert.Equal("room_abc", loaded.RoomCode);
        }
        finally
        {
            if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void PartyPacket_SerializesPulseWithTotalChestsCorrectly()
    {
        var packet = new PartyPacket
        {
            ClientId = "peer_client_1",
            Type = "pulse",
            RoomCode = "clan_alpha",
            PlayerName = "Ninja",
            MatchChests = 14,
            LastMatchChests = 14,
            SessionChests = 45,
            TotalChests = 320,
            MatchSeconds = 612
        };
        var json = JsonSerializer.Serialize(packet);
        var deserialized = JsonSerializer.Deserialize<PartyPacket>(json);

        Assert.NotNull(deserialized);
        Assert.Equal("pulse", deserialized.Type);
        Assert.Equal(14, deserialized.MatchChests);
        Assert.Equal(14, deserialized.LastMatchChests);
        Assert.Equal(45, deserialized.SessionChests);
        Assert.Equal(320, deserialized.TotalChests);
        Assert.Equal(612, deserialized.MatchSeconds);
        Assert.Equal("Ninja", deserialized.PlayerName);
    }

    [Fact]
    public void TeammateStats_PurelyInMemory_NeverSavedToDisk()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "LootPulse_RamTest_" + Guid.NewGuid().ToString("N"));
        try
        {
            var storage = new Storage(tempDir);
            var config = new Config { RoomCode = "ram_room", Profile = "LocalHost" };
            var data = new TrackerData { LifetimeChests = 100 };
            storage.SaveConfig(config);
            storage.SaveData(data);

            var hub = new PartyHub(config);
            var matePacket = new PartyPacket
            {
                ClientId = "remote_99",
                RoomCode = "ram_room",
                PlayerName = "ketamori",
                LastMatchChests = 50,
                TotalChests = 400,
                SessionChests = 200,
                SentUtc = DateTime.UtcNow
            };

            var method = typeof(PartyHub).GetMethod("ProcessIncomingJson", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(method);
            method.Invoke(hub, new object[] { JsonSerializer.Serialize(matePacket) });

            // Teammate is in RAM
            var peers = hub.GetConnectedPeers();
            Assert.Single(peers);
            Assert.Equal("ketamori", peers[0].PlayerName);
            Assert.Equal(50, peers[0].LastMatchChests);
            Assert.Equal(400, peers[0].TotalChests);
            Assert.Equal(200, peers[0].SessionChests);

            // Verify disk data is completely untouched and contains 0 teammate data
            var reloadedData = storage.LoadData();
            Assert.Equal(100, reloadedData.LifetimeChests);
            var jsonOnDisk = File.ReadAllText(Path.Combine(tempDir, "data.json"));
            Assert.DoesNotContain("ketamori", jsonOnDisk);
            Assert.DoesNotContain("400", jsonOnDisk);
            Assert.DoesNotContain("200", jsonOnDisk);
        }
        finally
        {
            if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
        }
    }
}

public class GuiAndStyleTests
{
    [Fact]
    public void GuiPage_ContainsZeroEmojis()
    {
        var html = GuiPage.Render("test_token_123");

        // Regex for emojis (standard Unicode emoji ranges)
        var emojiPattern = new Regex(@"[\uD83C-\uDBFF\uDC00-\uDFFF\u2600-\u27BF]", RegexOptions.Compiled);
        var matches = emojiPattern.Matches(html);

        Assert.True(matches.Count == 0, $"Found {matches.Count} emoji(s) in GUI HTML, but emojis are strictly forbidden!");
    }

    [Fact]
    public void GuiPage_EmbedsSvgIcons()
    {
        var html = GuiPage.Render("secure_token");

        Assert.Contains("<svg", html);
        Assert.Contains("LootPulse", html);
        Assert.Contains("secure_token", html);
    }

    [Fact]
    public void GuiPage_ContainsStatsTab_AndChartElements()
    {
        var html = GuiPage.Render("test_token");

        Assert.Contains("data-tab=\"tab-stats\"", html);
        Assert.Contains("Статистика", html);
        Assert.Contains("id=\"tab-stats\"", html);
        Assert.Contains("id=\"weeklyChartContainer\"", html);
        Assert.Contains("id=\"dayChestsPerHour\"", html);
        Assert.Contains("id=\"dayTimeInGame\"", html);
        Assert.Contains("id=\"dayAvgChests\"", html);
        Assert.Contains("id=\"dayTotalChests\"", html);
    }
}

public class ChestAnalyzerTests
{
    [Fact]
    public void At_InterpolatesContinuousPosition_WhenStationaryLongerThan15Seconds()
    {
        var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static;
        var ptType = typeof(ChestAnalyzer).GetNestedType("Pt", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        var atMethod = typeof(ChestAnalyzer).GetMethod("At", flags)!;

        // Player was at (X=100, Y=200, Z=0) at t=10s, and next movement was at t=40s (gap 30s) at (X=100, Y=200, Z=0)
        var listType = typeof(System.Collections.Generic.List<>).MakeGenericType(ptType);
        var track = (System.Collections.IList)Activator.CreateInstance(listType)!;
        track.Add(Activator.CreateInstance(ptType, 10f, 100f, 200f, 0f)!);
        track.Add(Activator.CreateInstance(ptType, 40f, 100f, 200f, 0f)!);

        // Check at t=25s (15s after t=10, 15s before t=40)
        var result = atMethod.Invoke(null, new object[] { track, 25f, 15f });
        Assert.NotNull(result);

        var xField = ptType.GetField("X") ?? ptType.GetProperty("X") as System.Reflection.MemberInfo;
        var yField = ptType.GetField("Y") ?? ptType.GetProperty("Y") as System.Reflection.MemberInfo;

        float x = (float)(ptType.GetProperty("X")?.GetValue(result) ?? ptType.GetField("X")?.GetValue(result)!);
        float y = (float)(ptType.GetProperty("Y")?.GetValue(result) ?? ptType.GetField("Y")?.GetValue(result)!);

        Assert.Equal(100f, x);
        Assert.Equal(200f, y);
    }

    [Fact]
    public void ResolveOrphans_AttributesBuildingCollapseChestsUpTo100Meters()
    {
        var analyzer = new ChestAnalyzer();
        var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;

        // Player pawn 101 at location (X=1000, Y=1000, Z=0) at time t=50s
        var pawnTrackField = typeof(ChestAnalyzer).GetField("_pawnTrack", flags)!;
        var pTrackDict = (System.Collections.IDictionary)pawnTrackField.GetValue(analyzer)!;

        var ptType = typeof(ChestAnalyzer).GetNestedType("Pt", flags)!;
        var pTrackList = (System.Collections.IList)Activator.CreateInstance(typeof(System.Collections.Generic.List<>).MakeGenericType(ptType))!;
        pTrackList.Add(Activator.CreateInstance(ptType, 50f, 1000f, 1000f, 0f)!);
        pTrackDict.Add(101u, pTrackList);

        var p2psField = typeof(ChestAnalyzer).GetField("_pawnToPlayerState", flags)!;
        var p2ps = (System.Collections.Generic.Dictionary<uint, uint>)p2psField.GetValue(analyzer)!;
        p2ps[101u] = 201u;

        // Chest 1: 30m away horizontally (3000 units), 40m up (4000 units) -> within 100m
        var lootLocField = typeof(ChestAnalyzer).GetField("_lootLocations", flags)!;
        var lootList = (System.Collections.IList)lootLocField.GetValue(analyzer)!;
        var lootTupleType = typeof(ValueTuple<float, uint, object>).Assembly.GetType("System.ValueTuple`3")!
            .MakeGenericType(typeof(float), typeof(uint), ptType);
        lootList.Add(Activator.CreateInstance(lootTupleType, 50f, 301u, Activator.CreateInstance(ptType, 50f, 4000f, 1000f, 4000f)!)!);

        // Chest 2: 80m away horizontally (8000 units), 20m up (2000 units) -> within 100m
        lootList.Add(Activator.CreateInstance(lootTupleType, 50f, 302u, Activator.CreateInstance(ptType, 50f, 9000f, 1000f, 2000f)!)!);

        // Chest 3: 150m away (15000 units) -> beyond 100m, must NOT be attributed
        lootList.Add(Activator.CreateInstance(lootTupleType, 50f, 303u, Activator.CreateInstance(ptType, 50f, 16000f, 1000f, 0f)!)!);

        // Add orphans
        var orphanType = typeof(ChestAnalyzer).GetNestedType("OrphanDestroy", flags)!;
        var orphanField = typeof(ChestAnalyzer).GetField("_orphans", flags)!;
        var orphanList = (System.Collections.IList)orphanField.GetValue(analyzer)!;

        orphanList.Add(Activator.CreateInstance(orphanType, 50f, "chest_floor2", "Tiered_Chest_Athena", 301u, 1u)!);
        orphanList.Add(Activator.CreateInstance(orphanType, 50f, "chest_floor4", "Tiered_Chest_Athena", 302u, 2u)!);
        orphanList.Add(Activator.CreateInstance(orphanType, 50f, "chest_far_away", "Tiered_Chest_Athena", 303u, 3u)!);

        // Invoke ResolveOrphans
        var resolveMethod = typeof(ChestAnalyzer).GetMethod("ResolveOrphans", flags)!;
        resolveMethod.Invoke(analyzer, null);

        // Check _searches
        var searchesField = typeof(ChestAnalyzer).GetField("_searches", flags)!;
        var searches = (System.Collections.IList)searchesField.GetValue(analyzer)!;

        // Chest 1 and Chest 2 must be attributed to pawn 101, Chest 3 must be excluded!
        Assert.Equal(2, searches.Count);

        var s0 = searches[0]!;
        var s1 = searches[1]!;

        var pProp = s0.GetType().GetProperty("SearcherPawn")!;
        var lProp = s0.GetType().GetProperty("Label")!;

        Assert.Equal(101u, (uint?)pProp.GetValue(s0));
        Assert.Equal("Tiered_Chest_Athena", (string?)lProp.GetValue(s0));

        Assert.Equal(101u, (uint?)pProp.GetValue(s1));
        Assert.Equal("Tiered_Chest_Athena", (string?)lProp.GetValue(s1));
    }

    [Fact]
    public void Updater_VersionParsing_AndComparison()
    {
        var vCurrent = Updater.ParseNorm("v2.0.0");
        var vNewer = Updater.ParseNorm("v2.1.0");
        var vSame = Updater.ParseNorm("2.0.0-beta");
        var vOlder = Updater.ParseNorm("v1.9.5");

        Assert.True(vNewer > vCurrent);
        Assert.Equal(vCurrent, vSame);
        Assert.True(vOlder < vCurrent);

        Assert.Equal("2.1.0", Updater.CleanVersion("v2.1.0-alpha.1"));
        Assert.Equal("2.0.0", Updater.CleanVersion("  2.0.0  "));
    }

    [Fact]
    public void GuiPage_ContainsUpdaterSettings_AndControls()
    {
        var html = GuiPage.Render("token_updater_check");

        Assert.Contains("id=\"currentVersionBadge\"", html);
        Assert.Contains("id=\"updateRepoInput\"", html);
        Assert.Contains("id=\"autoUpdateCheckbox\"", html);
        Assert.Contains("id=\"checkUpdateBtn\"", html);
        Assert.Contains("id=\"updateNoticeBox\"", html);
        Assert.Contains("id=\"applyUpdateBtn\"", html);
        Assert.Contains("checkUpdates", html);
        Assert.Contains("applyUpdate", html);
        Assert.Contains("id=\"onlyInGameCheckbox\"", html);
        Assert.Contains("toggleOnlyInGame", html);
    }

    [Fact]
    public void PoiResolver_ResolvesKnownLocationsCorrectly()
    {
        // Salty Springs near (-25700, -25200)
        Assert.Equal("Salty Springs", PoiResolver.Resolve(-25000, -25000));
        // The Agency near (0, -3500)
        Assert.Equal("The Agency", PoiResolver.Resolve(100, -3600));
        // Retail Row near (63900, 27600)
        Assert.Equal("Retail Row", PoiResolver.Resolve(64000, 27000));
        // Far off into the ocean or outskirts
        Assert.Equal("Окрестности", PoiResolver.Resolve(500000, -500000));
    }

    [Fact]
    public void MatchSummary_LocationsAndMainLocation_CalculatesCorrectly()
    {
        var match = new MatchSummary();
        Assert.Equal("Окрестности", match.MainLocation);

        match.Locations["Retail Row"] = 5;
        match.Locations["Salty Springs"] = 9;
        match.Locations["Pleasant Park"] = 2;

        Assert.Equal("Salty Springs", match.MainLocation);
        Assert.Equal(3, match.Locations.Count);
        Assert.Equal(9, match.Locations["Salty Springs"]);
    }

    [Fact]
    public void GuiPage_ContainsLocationAndPaceElements()
    {
        var html = GuiPage.Render("token_loc_check");

        // POI elements
        Assert.Contains("Популярные локации (Drop Spots)", html);
        Assert.Contains("id=\"topLocationsCard\"", html);
        Assert.Contains("id=\"topLocationsList\"", html);
        Assert.Contains("Локация", html);

        // CPM / Pace elements
        Assert.Contains("id=\"currentMatchCpm\"", html);
        Assert.Contains("Темп:", html);
    }

    [Fact]
    public void PartyHub_EnqueuesNotification_OnTeammateJoined()
    {
        var config = new Config { RoomCode = "notif_room", Profile = "Me" };
        var hub = new PartyHub(config);

        string? notifiedTeammate = null;
        hub.OnTeammateJoined = name => notifiedTeammate = name;

        var mate = new PartyPacket
        {
            ClientId = "peer_join_1",
            RoomCode = "notif_room",
            PlayerName = "FortniteGod99",
            MatchChests = 2,
            SentUtc = DateTime.UtcNow
        };

        var method = typeof(PartyHub).GetMethod("ProcessIncomingJson", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(method);
        method.Invoke(hub, new object[] { JsonSerializer.Serialize(mate) });

        Assert.Equal("FortniteGod99", notifiedTeammate);
        var alert = hub.DequeueNotification();
        Assert.NotNull(alert);
        Assert.Contains("FortniteGod99", alert);
    }
}
