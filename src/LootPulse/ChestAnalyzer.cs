using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FortniteReplayReader;
using FortniteReplayReader.Models;
using FortniteReplayReader.Models.NetFieldExports;
using FortniteReplayReader.Models.NetFieldExports.Items;
using FortniteReplayReader.Models.NetFieldExports.Vehicles;
using Unreal.Core;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace LootPulse;

public sealed class ChestAnalyzer : FortniteReplayReader.ReplayReader
{
    private sealed record SearchEvent(float Time, string ActorKey, string Label, uint? SearcherPawn, bool Destroyed = false);
    private readonly record struct Pt(float T, float X, float Y, float Z);

    private sealed class SeatSpan(uint pawn, uint vehicle, float from)
    {
        public uint Pawn { get; } = pawn;
        public uint Vehicle { get; } = vehicle;
        public float From { get; } = from;
        public float To { get; set; } = float.MaxValue;
    }

    private sealed record OrphanDestroy(float Time, string ActorKey, string Label, uint ActorId, uint ChannelIndex);

    private static readonly Regex VehicleClass = new(@"_Vehicle(_Static)?_C$|MeatballVehicle", RegexOptions.Compiled);
    private static readonly Regex IgnorePatterns = new(@"Deco|Tiki|Prop_|StreetLight|Traffic|Sign|Tree|Rock|Bush|Fence|Food|Ammo", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    static ChestAnalyzer()
    {
        NetFieldParser.AliasResolver = path =>
        {
            if (path.Contains(':') || path.EndsWith("_ClassNetCache", StringComparison.Ordinal))
            {
                return null;
            }
            var last = path[(path.LastIndexOf('/') + 1)..];
            if (VehicleClass.IsMatch(last)) return GenericVehicle.GroupPath;
            if (path.Contains("/Containers/", StringComparison.OrdinalIgnoreCase) || last.Contains("Chest", StringComparison.OrdinalIgnoreCase))
            {
                return GenericContainer.GroupPath;
            }
            return null;
        };
    }

    private readonly Dictionary<uint, string> _channelKey = new();
    private readonly Dictionary<uint, string> _channelLabel = new();
    private readonly Dictionary<uint, uint> _lastSearcher = new();
    private readonly HashSet<string> _searched = new();
    private readonly List<SearchEvent> _searches = new();
    private readonly Dictionary<uint, bool> _channelSearched = new();

    private readonly Dictionary<uint, Pt> _channelLocation = new();
    private readonly Dictionary<uint, Pt> _actorLocation = new();
    private readonly List<(float Time, uint Owner, Pt Loc)> _lootLocations = new();

    private readonly Dictionary<uint, List<Pt>> _pawnTrack = new();
    private readonly Dictionary<uint, List<Pt>> _vehicleTrack = new();
    private readonly List<SeatSpan> _seatSpans = new();
    private readonly Dictionary<(uint Vehicle, int Seat), SeatSpan> _seated = new();

    private readonly List<OrphanDestroy> _orphans = new();

    private readonly Dictionary<uint, uint> _pawnToPlayerState = new();
    private readonly Dictionary<uint, int> _playerStateToPlayerId = new();

    public ChestAnalyzer() : base(null, ParseMode.Normal) { }

    private string PathOf(uint? guid)
    {
        if (guid is null || guid == 0) return "";
        return _netGuidCache.TryGetPathName(guid.Value, out var path) ? path : "";
    }

    public MatchSummary Analyze(Stream stream, string fileName, string? targetProfile)
    {
        var replay = ReadReplay(stream);
        var players = replay.PlayerData?.ToList() ?? new List<PlayerData>();

        PlayerData? target = null;
        if (!string.IsNullOrWhiteSpace(targetProfile))
        {
            var names = targetProfile.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            target = players.FirstOrDefault(p => names.Any(n => string.Equals(p.PlayerName?.Trim(), n, StringComparison.OrdinalIgnoreCase)));
        }

        uint? recorderPs = replay.GameData?.RecorderId;
        if (target == null && recorderPs != null && _playerStateToPlayerId.TryGetValue(recorderPs.Value, out var recId))
        {
            target = players.FirstOrDefault(p => p.Id == recId);
        }

        var targetPlayerStates = new HashSet<uint>();
        if (target?.Id != null)
        {
            foreach (var kv in _playerStateToPlayerId.Where(kv => kv.Value == target.Id.Value))
            {
                targetPlayerStates.Add(kv.Key);
            }
        }
        if (targetPlayerStates.Count == 0 && recorderPs != null)
        {
            targetPlayerStates.Add(recorderPs.Value);
        }

        var targetPawns = _pawnToPlayerState.Where(kv => targetPlayerStates.Contains(kv.Value)).Select(kv => kv.Key).ToHashSet();

        ResolveOrphans();

        int opened = 0;
        int destroyed = 0;
        var events = new List<ChestEvent>();
        var locations = new Dictionary<string, int>();

        foreach (var s in _searches.OrderBy(s => s.Time))
        {
            if (s.SearcherPawn is uint p && targetPawns.Contains(p) && IsChest(s.Label))
            {
                var pPt = PositionOf(p, s.Time);
                var loc = pPt != null ? PoiResolver.Resolve(pPt.Value.X, pPt.Value.Y) : "Окрестности";
                locations[loc] = locations.GetValueOrDefault(loc, 0) + 1;

                if (s.Destroyed)
                {
                    destroyed++;
                    events.Add(new ChestEvent { TimeSeconds = s.Time, IsDestroyed = true, Label = s.Label, Location = loc });
                }
                else
                {
                    opened++;
                    events.Add(new ChestEvent { TimeSeconds = s.Time, IsDestroyed = false, Label = s.Label, Location = loc });
                }
            }
        }

        var duration = (int)(replay.Info?.LengthInMs / 1000 ?? 0);
        if (duration <= 0 && _searches.Count > 0)
        {
            duration = (int)_searches.Max(s => s.Time);
        }

        return new MatchSummary
        {
            SessionId = replay.GameData?.GameSessionId ?? Path.GetFileNameWithoutExtension(fileName),
            ReplayFile = fileName,
            DateUtc = replay.GameData?.UtcTimeStartedMatch ?? DateTime.UtcNow,
            DurationSeconds = Math.Max(0, duration),
            ChestsOpened = opened,
            ChestsDestroyed = destroyed,
            Events = events,
            Locations = locations
        };
    }

    private static bool IsChest(string label)
    {
        if (string.IsNullOrWhiteSpace(label)) return false;
        if (IgnorePatterns.IsMatch(label)) return false;
        return label.Contains("Chest", StringComparison.OrdinalIgnoreCase) || label.Contains("Coffre", StringComparison.OrdinalIgnoreCase);
    }

    private void ResolveOrphans()
    {
        // 100 meters = 10,000 Unreal Units (cm)
        const float maxHorizDist = 10000f; // 100m horizontal radius
        const float maxHorizDistSq = maxHorizDist * maxHorizDist; // 100_000_000 cm^2
        const float maxVertDist = 10000f; // 100m vertical (covers tall multi-story buildings and towers)

        foreach (var o in _orphans)
        {
            if (!IsChest(o.Label)) continue;

            // 1. Resolve chest 3D world location:
            // - Preferred: Loot drop position (FortPickup LootInitialPosition)
            // - Secondary: Container Actor Location
            // - Tertiary: Channel Location
            Pt? chestLoc = null;
            var loots = _lootLocations
                .Where(l => l.Owner == o.ActorId && Math.Abs(l.Time - o.Time) <= 4f)
                .Select(l => l.Loc)
                .ToList();

            if (loots.Count > 0)
            {
                chestLoc = loots[0];
            }
            else if (_actorLocation.TryGetValue(o.ActorId, out var aloc))
            {
                chestLoc = aloc;
            }
            else if (_channelLocation.TryGetValue(o.ChannelIndex, out var cloc))
            {
                chestLoc = cloc;
            }

            uint? bestPawn = null;
            float bestDist3DSq = float.MaxValue;

            if (chestLoc is { } cPt)
            {
                foreach (var pawn in _pawnToPlayerState.Keys)
                {
                    var pPt = PositionOf(pawn, o.Time);
                    if (pPt is null) continue;

                    var dx = pPt.Value.X - cPt.X;
                    var dy = pPt.Value.Y - cPt.Y;
                    var dz = pPt.Value.Z - cPt.Z;
                    var horizSq = dx * dx + dy * dy;

                    if (horizSq <= maxHorizDistSq && Math.Abs(dz) <= maxVertDist)
                    {
                        var dist3DSq = horizSq + dz * dz;
                        if (dist3DSq < bestDist3DSq)
                        {
                            bestDist3DSq = dist3DSq;
                            bestPawn = pawn;
                        }
                    }
                }
            }

            if (bestPawn != null)
            {
                _searches.Add(new SearchEvent(o.Time, o.ActorKey, o.Label, bestPawn, Destroyed: true));
            }
        }
        _orphans.Clear();
    }

    private Pt? PositionOf(uint pawn, float t)
    {
        foreach (var span in _seatSpans)
        {
            if (span.Pawn == pawn && t >= span.From - 1.0f && t <= span.To + 1.0f)
            {
                if (At(_vehicleTrack.GetValueOrDefault(span.Vehicle), t, 15f) is { } inVehicle)
                {
                    return inVehicle;
                }
            }
        }
        return At(_pawnTrack.GetValueOrDefault(pawn), t, 15f);
    }

    private static Pt? At(List<Pt>? track, float t, float maxGap = 15f)
    {
        if (track == null || track.Count == 0) return null;
        int lo = 0, hi = track.Count - 1;
        while (lo < hi)
        {
            var mid = (lo + hi) / 2;
            if (track[mid].T < t) lo = mid + 1; else hi = mid;
        }
        var after = track[lo];
        if (lo == 0)
        {
            return Math.Abs(after.T - t) <= maxGap ? after : null;
        }
        if (after.T <= t)
        {
            return Math.Abs(t - after.T) <= maxGap ? after : null;
        }
        var before = track[lo - 1];
        var dt = after.T - before.T;
        if (dt > 0.001f)
        {
            var f = (t - before.T) / dt;
            return new Pt(t, before.X + (after.X - before.X) * f, before.Y + (after.Y - before.Y) * f, before.Z + (after.Z - before.Z) * f);
        }
        return before;
    }

    protected override void OnChannelOpened(uint channelIndex, NetworkGUID? actor)
    {
        base.OnChannelOpened(channelIndex, actor);
        var channel = Channels[channelIndex];
        var archetype = PathOf(channel?.ArchetypeId);
        var actorPath = PathOf(channel?.ActorId);
        var label = !string.IsNullOrEmpty(archetype) ? archetype : actorPath ?? "";

        _channelLabel[channelIndex] = label;
        _channelKey[channelIndex] = !string.IsNullOrEmpty(actorPath) ? actorPath : $"guid:{channel?.ActorId}";
        _channelSearched[channelIndex] = false;

        if (channel?.Actor?.Location is { } loc)
        {
            var pt = new Pt(Now, (float)loc.X, (float)loc.Y, (float)loc.Z);
            _channelLocation[channelIndex] = pt;
            if (channel.ActorId != null)
            {
                _actorLocation[channel.ActorId.Value] = pt;
            }
        }
    }

    protected override void OnChannelClosed(uint channelIndex, NetworkGUID? actor)
    {
        _channelLabel.Remove(channelIndex);
        _channelKey.Remove(channelIndex);
        _channelSearched.Remove(channelIndex);
        _channelLocation.Remove(channelIndex);
        _lastSearcher.Remove(channelIndex);
        base.OnChannelClosed(channelIndex, actor);
    }

    protected override void OnExportRead(uint channelIndex, INetFieldExportGroup? exportGroup)
    {
        base.OnExportRead(channelIndex, exportGroup);

        switch (exportGroup)
        {
            case BaseContainer container:
                var cKey = _channelKey.GetValueOrDefault(channelIndex, $"ch:{channelIndex}");
                var cLabel = _channelLabel.GetValueOrDefault(channelIndex, "");
                var actorId = Channels[channelIndex]?.ActorId ?? channelIndex;

                if (container.bAlreadySearched == true)
                {
                    var was = _channelSearched.GetValueOrDefault(channelIndex, false);
                    _channelSearched[channelIndex] = true;
                    if (!was && _searched.Add(cKey))
                    {
                        uint? searcher = container.SearchingPawn ?? _lastSearcher.GetValueOrDefault(channelIndex);
                        _searches.Add(new SearchEvent(Now, cKey, cLabel, searcher));
                    }
                }
                else if (container.bDestroyed)
                {
                    var was = _channelSearched.GetValueOrDefault(channelIndex, false);
                    _channelSearched[channelIndex] = true;

                    if (!was && _searched.Add(cKey))
                    {
                        uint? searcher = container.SearchingPawn ?? _lastSearcher.GetValueOrDefault(channelIndex);
                        if (searcher != null && searcher != 0)
                        {
                            _searches.Add(new SearchEvent(Now, cKey, cLabel, searcher, Destroyed: true));
                        }
                        else
                        {
                            _orphans.Add(new OrphanDestroy(Now, cKey, cLabel, actorId, channelIndex));
                        }
                    }
                }
                if (container.SearchingPawn != null && container.SearchingPawn != 0)
                {
                    _lastSearcher[channelIndex] = container.SearchingPawn.Value;
                }
                break;

            case FortPickup pickup when pickup.bTossedFromContainer == true:
                if (pickup.Owner is uint cOwner && cOwner != 0)
                {
                    var loc = pickup.LootInitialPosition;
                    if (loc != null && (loc.X != 0 || loc.Y != 0 || loc.Z != 0))
                    {
                        _lootLocations.Add((Now, cOwner, new Pt(Now, (float)loc.X, (float)loc.Y, (float)loc.Z)));
                    }
                    else if (Channels[channelIndex]?.Actor?.Location is { } cloc)
                    {
                        _lootLocations.Add((Now, cOwner, new Pt(Now, (float)cloc.X, (float)cloc.Y, (float)cloc.Z)));
                    }
                    else if (pickup.ReplicatedMovement?.Location is { } rloc)
                    {
                        _lootLocations.Add((Now, cOwner, new Pt(Now, (float)rloc.X, (float)rloc.Y, (float)rloc.Z)));
                    }
                }
                break;

            case SeatComponent seats when seats.PlayerSlots != null && Channels[channelIndex]?.ActorId is uint seatVehicle:
                for (var seat = 0; seat < seats.PlayerSlots.Length; seat++)
                {
                    if (seats.PlayerSlots[seat]?.Player is not uint occupant) continue;
                    if (_seated.Remove((seatVehicle, seat), out var leaving))
                    {
                        leaving.To = Now;
                    }
                    if (occupant != 0)
                    {
                        var span = new SeatSpan(occupant, seatVehicle, Now);
                        _seated[(seatVehicle, seat)] = span;
                        _seatSpans.Add(span);
                    }
                }
                break;

            case FortPlayerState state:
                var psId = Channels[channelIndex]?.ActorId;
                var pId = state.PlayerId is null ? state.PlayerID : (int?)state.PlayerId;
                if (psId != null && pId != null)
                {
                    _playerStateToPlayerId[psId.Value] = pId.Value;
                }
                break;

            case PlayerPawn pawn:
                var pawnId = Channels[channelIndex]?.ActorId;
                if (pawnId != null)
                {
                    if (pawn.PlayerState is uint ps && ps != 0)
                    {
                        _pawnToPlayerState[pawnId.Value] = ps;
                    }
                    if (pawn.ReplicatedMovement?.Location is { } loc)
                    {
                        if (!_pawnTrack.TryGetValue(pawnId.Value, out var tr)) _pawnTrack[pawnId.Value] = tr = new();
                        tr.Add(new Pt(Now, (float)loc.X, (float)loc.Y, (float)loc.Z));
                    }
                }
                break;

            case GenericVehicle gv:
                if (gv.ReplicatedMovement?.Location is { } vloc && Channels[channelIndex]?.ActorId is uint vehId)
                {
                    if (!_vehicleTrack.TryGetValue(vehId, out var tr)) _vehicleTrack[vehId] = tr = new();
                    tr.Add(new Pt(Now, (float)vloc.X, (float)vloc.Y, (float)vloc.Z));
                }
                break;
        }
    }

    private static float Now => Unreal.Core.ReplayReader<FortniteReplay>.CurrentTime;
}
