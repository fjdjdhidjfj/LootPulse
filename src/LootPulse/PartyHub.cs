using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LootPulse;

public sealed class PartyHub
{
    private const int UdpPort = 38292;
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(8) };
    private static readonly string[] RelayHosts = ["https://ntfy.tedomum.fr", "https://ntfy.adminforge.de"];

    public static readonly string LocalClientId = Guid.NewGuid().ToString("N")[..8];

    private static readonly Lazy<UdpClient> BroadcastClient = new(() =>
    {
        var client = new UdpClient();
        try { client.EnableBroadcast = true; } catch { /* ignore */ }
        return client;
    });

    private readonly ConcurrentDictionary<string, TeammatePeer> _peers = new(StringComparer.OrdinalIgnoreCase);
    private CancellationTokenSource? _cts;
    private MqttRelay? _mqttRelay;
    private Config _config;
    private string? _lastStateKey;
    private DateTime _lastHttpSentUtc = DateTime.MinValue;
    private int _httpSending;

    public Action? OnRosterChanged { get; set; }

    public PartyHub(Config config)
    {
        _config = config;
    }

    public void UpdateConfig(Config config)
    {
        var codeChanged = !string.Equals(_config.RoomCode, config.RoomCode, StringComparison.OrdinalIgnoreCase);
        _config = config;
        if (codeChanged)
        {
            _peers.Clear();
            Restart();
            OnRosterChanged?.Invoke();
        }
    }

    public void Start() => Restart();

    public void Stop()
    {
        _cts?.Cancel();
        _cts = null;
        _mqttRelay?.Dispose();
        _mqttRelay = null;
    }

    private void Restart()
    {
        Stop();
        if (string.IsNullOrWhiteSpace(_config.RoomCode)) return;

        _cts = new CancellationTokenSource();
        var ct = _cts.Token;
        var roomHash = HashRoom(_config.RoomCode);

        // 1. Ultra low-latency MQTT Relay (primary)
        _mqttRelay = new MqttRelay(roomHash, json => ProcessIncomingJson(json));
        _mqttRelay.Start();

        // 2. LAN UDP listener
        Task.Run(async () =>
        {
            try
            {
                using var udp = new UdpClient();
                udp.Client.ExclusiveAddressUse = false;
                udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                udp.Client.Bind(new IPEndPoint(IPAddress.Any, UdpPort));

                while (!ct.IsCancellationRequested)
                {
                    try
                    {
                        var res = await udp.ReceiveAsync(ct);
                        var json = Encoding.UTF8.GetString(res.Buffer).Trim();
                        ProcessIncomingJson(json);
                    }
                    catch (OperationCanceledException) { break; }
                    catch { /* ignore */ }
                }
            }
            catch { /* UDP bind error */ }
        }, ct);

        // 3. Internet Relay Listener (Stream + Fallback Polling)
        // Live SSE Stream
        Task.Run(async () =>
        {
            while (!ct.IsCancellationRequested)
            {
                foreach (var host in RelayHosts)
                {
                    if (ct.IsCancellationRequested) break;
                    try
                    {
                        var url = $"{host}/lp2_{roomHash}/json";
                        using var req = new HttpRequestMessage(HttpMethod.Get, url);
                        req.Headers.TryAddWithoutValidation("User-Agent", "LootPulse/2.0");
                        using var resp = await Http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);

                        if (resp.IsSuccessStatusCode)
                        {
                            using var stream = await resp.Content.ReadAsStreamAsync(ct);
                            using var reader = new StreamReader(stream);
                            while (!ct.IsCancellationRequested && await reader.ReadLineAsync(ct) is { } line)
                            {
                                if (string.IsNullOrWhiteSpace(line)) continue;
                                try
                                {
                                    using var doc = JsonDocument.Parse(line);
                                    if (doc.RootElement.TryGetProperty("event", out var ev) && ev.GetString() == "message"
                                        && doc.RootElement.TryGetProperty("message", out var msg))
                                    {
                                        ProcessIncomingJson(msg.GetString() ?? "");
                                    }
                                }
                                catch { /* skip non-json line */ }
                            }
                        }
                    }
                    catch (OperationCanceledException) { break; }
                    catch
                    {
                        await Task.Delay(3000, ct).ConfigureAwait(false);
                    }
                }
            }
        }, ct);

        // Polling loop as backup
        Task.Run(async () =>
        {
            while (!ct.IsCancellationRequested)
            {
                await Task.Delay(4000, ct).ConfigureAwait(false);
                foreach (var host in RelayHosts)
                {
                    try
                    {
                        var pollUrl = $"{host}/lp2_{roomHash}/json?poll=1&since=25s";
                        using var req = new HttpRequestMessage(HttpMethod.Get, pollUrl);
                        req.Headers.TryAddWithoutValidation("User-Agent", "LootPulse/2.0");
                        using var resp = await Http.SendAsync(req, ct);
                        if (resp.IsSuccessStatusCode)
                        {
                            var content = await resp.Content.ReadAsStringAsync(ct);
                            foreach (var line in content.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                            {
                                try
                                {
                                    using var doc = JsonDocument.Parse(line);
                                    if (doc.RootElement.TryGetProperty("event", out var ev) && ev.GetString() == "message"
                                        && doc.RootElement.TryGetProperty("message", out var msg))
                                    {
                                        ProcessIncomingJson(msg.GetString() ?? "");
                                    }
                                }
                                catch { /* skip */ }
                            }
                            break; // success on this host
                        }
                    }
                    catch { /* try next host */ }
                }
            }
        }, ct);

        // 3. Heartbeat cleanup timer (remove offline peers after 60s)
        Task.Run(async () =>
        {
            while (!ct.IsCancellationRequested)
            {
                await Task.Delay(4000, ct).ConfigureAwait(false);
                var expired = _peers.Where(kv => !kv.Value.IsOnline).Select(kv => kv.Key).ToList();
                if (expired.Count > 0)
                {
                    foreach (var k in expired) _peers.TryRemove(k, out _);
                    OnRosterChanged?.Invoke();
                }
            }
        }, ct);
    }

    public void BroadcastPulse(int matchChests, int sessionChests, int totalChests, int lastMatchChests, int matchSeconds)
    {
        if (string.IsNullOrWhiteSpace(_config.RoomCode)) return;

        var myName = !string.IsNullOrWhiteSpace(_config.Profile) ? _config.Profile : Environment.MachineName;
        var packet = new PartyPacket
        {
            ClientId = LocalClientId,
            Type = "pulse",
            RoomCode = _config.RoomCode.Trim(),
            PlayerName = myName,
            MatchChests = matchChests,
            LastMatchChests = lastMatchChests,
            SessionChests = sessionChests,
            TotalChests = totalChests,
            MatchSeconds = matchSeconds,
            SentUtc = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(packet);
        var bytes = Encoding.UTF8.GetBytes(json);

        // 1. LAN Broadcast across interfaces (reuses single socket)
        try
        {
            BroadcastClient.Value.Send(bytes, bytes.Length, new IPEndPoint(IPAddress.Broadcast, UdpPort));
        }
        catch { /* ignore */ }

        // 2. Ultra-fast real-time MQTT delivery
        _mqttRelay?.Publish(json);

        // 3. Internet Relay Fallback (Throttled: immediate on state change, 8s on idle heartbeat)
        var stateKey = $"{matchChests}:{lastMatchChests}:{sessionChests}:{totalChests}";
        var isChanged = !string.Equals(stateKey, _lastStateKey, StringComparison.Ordinal);
        var elapsed = DateTime.UtcNow - _lastHttpSentUtc;

        if (!isChanged && elapsed < TimeSpan.FromSeconds(8))
        {
            return;
        }

        if (Interlocked.CompareExchange(ref _httpSending, 1, 0) == 0)
        {
            _lastStateKey = stateKey;
            _lastHttpSentUtc = DateTime.UtcNow;

            var roomHash = HashRoom(_config.RoomCode);
            Task.Run(async () =>
            {
                try
                {
                    foreach (var host in RelayHosts)
                    {
                        try
                        {
                            using var content = new StringContent(json, Encoding.UTF8, "text/plain");
                            using var req = new HttpRequestMessage(HttpMethod.Post, $"{host}/lp2_{roomHash}") { Content = content };
                            req.Headers.TryAddWithoutValidation("User-Agent", "LootPulse/2.0");
                            var resp = await Http.SendAsync(req);
                            if (resp.IsSuccessStatusCode) break;
                        }
                        catch { /* fallback to next host */ }
                    }
                }
                finally
                {
                    Interlocked.Exchange(ref _httpSending, 0);
                }
            });
        }
    }

    private void ProcessIncomingJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json) || !json.StartsWith('{')) return;

        try
        {
            var packet = JsonSerializer.Deserialize<PartyPacket>(json);
            if (packet == null) return;

            // Strict Room check
            if (!string.Equals(packet.RoomCode, _config.RoomCode?.Trim(), StringComparison.OrdinalIgnoreCase)) return;

            // Reject self-echo by unique ClientId
            if (string.Equals(packet.ClientId, LocalClientId, StringComparison.OrdinalIgnoreCase)) return;

            // Reject self-echo if same machine and profile
            var myName = !string.IsNullOrWhiteSpace(_config.Profile) ? _config.Profile : Environment.MachineName;
            if (string.Equals(packet.PlayerName, myName, StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(packet.ClientId)) return;

            // Timestamp sanity (10 minutes tolerance for desynced PC clocks)
            if (Math.Abs((DateTime.UtcNow - packet.SentUtc).TotalMinutes) > 10) return;

            // Deduplicate peers by PlayerName (clean reconnects without ghost rows)
            var peerKey = !string.IsNullOrWhiteSpace(packet.PlayerName)
                ? packet.PlayerName.Trim().ToLowerInvariant()
                : (!string.IsNullOrEmpty(packet.ClientId) ? packet.ClientId : "unknown");

            var peer = _peers.GetOrAdd(peerKey, _ => new TeammatePeer { ClientId = packet.ClientId, PlayerName = packet.PlayerName });
            peer.ClientId = packet.ClientId;
            peer.PlayerName = packet.PlayerName;
            peer.RoomCode = packet.RoomCode;
            peer.MatchChests = packet.MatchChests;
            peer.LastMatchChests = packet.LastMatchChests;
            peer.SessionChests = packet.SessionChests;
            peer.TotalChests = packet.TotalChests;
            peer.MatchSeconds = packet.MatchSeconds;
            peer.LastSeenUtc = DateTime.UtcNow;

            OnRosterChanged?.Invoke();
        }
        catch { /* invalid packet */ }
    }

    public IReadOnlyList<TeammatePeer> GetConnectedPeers() => _peers.Values.Where(p => p.IsOnline).ToList();

    public int GetSquadMatchChests() => _peers.Values.Where(p => p.IsOnline).Sum(p => p.MatchChests);

    public int GetSquadSessionChests() => _peers.Values.Where(p => p.IsOnline).Sum(p => p.SessionChests);

    private static string HashRoom(string room)
    {
        var raw = room.Trim().ToLowerInvariant();
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(raw + "_lootpulse_v2"));
        return Convert.ToHexString(hash, 0, 10).ToLowerInvariant();
    }

    private sealed class MqttRelay : IDisposable
    {
        private static readonly (string host, int port)[] Brokers = [("broker.emqx.io", 1883), ("broker.hivemq.com", 1883)];
        private readonly string _topic;
        private readonly Action<string> _onMessage;
        private readonly object _streamLock = new();
        private TcpClient? _tcp;
        private NetworkStream? _stream;
        private CancellationTokenSource? _cts;

        public MqttRelay(string roomHash, Action<string> onMessage)
        {
            _topic = $"lootpulse/v2/{roomHash}";
            _onMessage = onMessage;
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            var ct = _cts.Token;
            Task.Run(() => WorkerLoop(ct), ct);
        }

        private async Task WorkerLoop(CancellationToken ct)
        {
            int brokerIndex = 0;
            while (!ct.IsCancellationRequested)
            {
                var (host, port) = Brokers[brokerIndex % Brokers.Length];
                brokerIndex++;
                try
                {
                    _tcp = new TcpClient();
                    await _tcp.ConnectAsync(host, port, ct).ConfigureAwait(false);
                    var s = _tcp.GetStream();

                    var clientId = "lp_" + Guid.NewGuid().ToString("N")[..8];
                    var connPacket = BuildConnectPacket(clientId);
                    await s.WriteAsync(connPacket, ct).ConfigureAwait(false);

                    var ack = new byte[4];
                    int read = await s.ReadAsync(ack.AsMemory(0, 4), ct).ConfigureAwait(false);
                    if (read < 4 || ack[0] != 0x20 || ack[3] != 0x00)
                    {
                        throw new IOException("MQTT CONNACK failed");
                    }

                    var subPacket = BuildSubscribePacket(_topic, 1);
                    await s.WriteAsync(subPacket, ct).ConfigureAwait(false);
                    _stream = s;

                    _ = Task.Run(async () =>
                    {
                        while (!ct.IsCancellationRequested && _tcp?.Connected == true)
                        {
                            await Task.Delay(20000, ct).ConfigureAwait(false);
                            try
                            {
                                var s = _stream;
                                if (s != null)
                                {
                                    lock (_streamLock)
                                    {
                                        s.Write(new byte[] { 0xC0, 0x00 }, 0, 2);
                                    }
                                }
                            }
                            catch { break; }
                        }
                    }, ct);

                    while (!ct.IsCancellationRequested && _tcp.Connected)
                    {
                        int header = _stream.ReadByte();
                        if (header < 0) break;

                        int len = ReadVarInt(_stream);
                        if (len <= 0 || len > 65536) continue;

                        var payload = new byte[len];
                        int total = 0;
                        while (total < len)
                        {
                            int r = await _stream.ReadAsync(payload.AsMemory(total, len - total), ct).ConfigureAwait(false);
                            if (r <= 0) break;
                            total += r;
                        }
                        if (total < len) break;

                        if ((header & 0xF0) == 0x30)
                        {
                            int topicLen = (payload[0] << 8) | payload[1];
                            int offset = 2 + topicLen;
                            if ((header & 0x06) != 0) offset += 2;
                            if (offset < len)
                            {
                                var json = Encoding.UTF8.GetString(payload, offset, len - offset);
                                _onMessage(json);
                            }
                        }
                    }
                }
                catch (OperationCanceledException) { break; }
                catch { /* try next broker on disconnect */ }
                finally
                {
                    try { _stream?.Dispose(); } catch { }
                    try { _tcp?.Dispose(); } catch { }
                }

                if (!ct.IsCancellationRequested)
                {
                    await Task.Delay(3000, ct).ConfigureAwait(false);
                }
            }
        }

        public void Publish(string message)
        {
            try
            {
                var s = _stream;
                if (s != null && _tcp?.Connected == true)
                {
                    var pubPacket = BuildPublishPacket(_topic, message);
                    lock (_streamLock)
                    {
                        s.Write(pubPacket, 0, pubPacket.Length);
                    }
                }
            }
            catch { /* best effort */ }
        }

        private static byte[] BuildConnectPacket(string clientId)
        {
            using var ms = new MemoryStream();
            ms.WriteByte(0x00); ms.WriteByte(0x04);
            ms.Write(Encoding.ASCII.GetBytes("MQTT"));
            ms.WriteByte(0x04);
            ms.WriteByte(0x02);
            ms.WriteByte(0x00); ms.WriteByte(0x3C);
            var idBytes = Encoding.UTF8.GetBytes(clientId);
            ms.WriteByte((byte)(idBytes.Length >> 8));
            ms.WriteByte((byte)(idBytes.Length & 0xFF));
            ms.Write(idBytes);

            var body = ms.ToArray();
            var packet = new List<byte> { 0x10 };
            WriteVarInt(packet, body.Length);
            packet.AddRange(body);
            return packet.ToArray();
        }

        private static byte[] BuildSubscribePacket(string topic, ushort packetId)
        {
            using var ms = new MemoryStream();
            ms.WriteByte((byte)(packetId >> 8));
            ms.WriteByte((byte)(packetId & 0xFF));
            var tBytes = Encoding.UTF8.GetBytes(topic);
            ms.WriteByte((byte)(tBytes.Length >> 8));
            ms.WriteByte((byte)(tBytes.Length & 0xFF));
            ms.Write(tBytes);
            ms.WriteByte(0x00);

            var body = ms.ToArray();
            var packet = new List<byte> { 0x82 };
            WriteVarInt(packet, body.Length);
            packet.AddRange(body);
            return packet.ToArray();
        }

        private static byte[] BuildPublishPacket(string topic, string message)
        {
            var tBytes = Encoding.UTF8.GetBytes(topic);
            var mBytes = Encoding.UTF8.GetBytes(message);
            var bodyLen = 2 + tBytes.Length + mBytes.Length;

            var packet = new List<byte> { 0x30 };
            WriteVarInt(packet, bodyLen);
            packet.Add((byte)(tBytes.Length >> 8));
            packet.Add((byte)(tBytes.Length & 0xFF));
            packet.AddRange(tBytes);
            packet.AddRange(mBytes);
            return packet.ToArray();
        }

        private static int ReadVarInt(Stream stream)
        {
            int multiplier = 1, value = 0;
            int digit;
            do
            {
                digit = stream.ReadByte();
                if (digit < 0) return -1;
                value += (digit & 127) * multiplier;
                multiplier *= 128;
            } while ((digit & 128) != 0);
            return value;
        }

        private static void WriteVarInt(List<byte> bytes, int val)
        {
            do
            {
                byte d = (byte)(val % 128);
                val /= 128;
                if (val > 0) d |= 128;
                bytes.Add(d);
            } while (val > 0);
        }

        public void Dispose()
        {
            _cts?.Cancel();
            try { _stream?.Dispose(); } catch { }
            try { _tcp?.Dispose(); } catch { }
        }
    }
}
