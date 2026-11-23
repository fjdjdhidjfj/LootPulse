using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LootPulse;

public sealed record UpdateCheckResult
{
    public bool HasUpdate { get; init; }
    public string CurrentVersion { get; init; } = "";
    public string LatestVersion { get; init; } = "";
    public string DownloadUrl { get; init; } = "";
    public long FileSizeBytes { get; init; }
    public string Changelog { get; init; } = "";
    public string? ErrorMessage { get; init; }
}

public static class Updater
{
    private static readonly HttpClient Http = new();

    static Updater()
    {
        Http.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "LootPulse-Updater/2.0");
        Http.Timeout = TimeSpan.FromSeconds(10);
    }

    public static async Task<UpdateCheckResult> CheckAsync(string repoOrUrl, string currentVersion, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(repoOrUrl))
        {
            return new UpdateCheckResult
            {
                CurrentVersion = currentVersion,
                ErrorMessage = "Репозиторий обновлений не указан"
            };
        }

        try
        {
            string url;
            repoOrUrl = repoOrUrl.Trim();
            if (repoOrUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || repoOrUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                url = repoOrUrl;
            }
            else
            {
                var parts = repoOrUrl.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (parts.Length != 2)
                {
                    return new UpdateCheckResult
                    {
                        CurrentVersion = currentVersion,
                        ErrorMessage = "Формат должен быть 'владелец/репозиторий' (например: fjdjdhidjfj/LootPulse)"
                    };
                }
                url = $"https://api.github.com/repos/{parts[0]}/{parts[1]}/releases/latest";
            }

            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            using var resp = await Http.SendAsync(req, ct).ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode)
            {
                return new UpdateCheckResult
                {
                    CurrentVersion = currentVersion,
                    ErrorMessage = $"Сервер вернул ошибку {(int)resp.StatusCode}: {resp.ReasonPhrase}"
                };
            }

            using var doc = await JsonDocument.ParseAsync(await resp.Content.ReadAsStreamAsync(ct).ConfigureAwait(false), cancellationToken: ct).ConfigureAwait(false);
            var root = doc.RootElement;

            string tagName = root.TryGetProperty("tag_name", out var tagElem) ? tagElem.GetString() ?? "" : "";
            string body = root.TryGetProperty("body", out var bodyElem) ? bodyElem.GetString() ?? "" : "";
            string downloadUrl = "";
            long size = 0;

            if (root.TryGetProperty("assets", out var assetsElem) && assetsElem.ValueKind == JsonValueKind.Array)
            {
                foreach (var asset in assetsElem.EnumerateArray())
                {
                    var name = asset.TryGetProperty("name", out var ne) ? ne.GetString() ?? "" : "";
                    if (name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                    {
                        downloadUrl = asset.TryGetProperty("browser_download_url", out var ue) ? ue.GetString() ?? "" : "";
                        size = asset.TryGetProperty("size", out var se) ? se.GetInt64() : 0;
                        if (name.Equals("LootPulse.exe", StringComparison.OrdinalIgnoreCase)) break;
                    }
                }
            }

            var remoteNorm = ParseNorm(tagName);
            var currentNorm = ParseNorm(currentVersion);
            bool hasUpdate = remoteNorm > currentNorm && !string.IsNullOrEmpty(downloadUrl);

            return new UpdateCheckResult
            {
                HasUpdate = hasUpdate,
                CurrentVersion = currentVersion,
                LatestVersion = CleanVersion(tagName),
                DownloadUrl = downloadUrl,
                FileSizeBytes = size,
                Changelog = body
            };
        }
        catch (Exception ex)
        {
            return new UpdateCheckResult
            {
                CurrentVersion = currentVersion,
                ErrorMessage = ex.Message
            };
        }
    }

    public static async Task ApplyAsync(string downloadUrl, Action<int>? onProgress = null, CancellationToken ct = default)
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"LootPulse_Update_{Guid.NewGuid():N}.exe");
        using var client = new HttpClient();
        client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "LootPulse-Updater/2.0");

        using (var resp = await client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false))
        {
            resp.EnsureSuccessStatusCode();
            var totalBytes = resp.Content.Headers.ContentLength ?? -1L;
            using var stream = await resp.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
            using var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None);

            var buf = new byte[81920];
            long totalRead = 0;
            int read;
            while ((read = await stream.ReadAsync(buf.AsMemory(0, buf.Length), ct).ConfigureAwait(false)) > 0)
            {
                await fs.WriteAsync(buf.AsMemory(0, read), ct).ConfigureAwait(false);
                totalRead += read;
                if (totalBytes > 0 && onProgress != null)
                {
                    onProgress((int)((totalRead * 100) / totalBytes));
                }
            }
        }

        // Validate PE executable header (MZ = 0x4D, 0x5A)
        var header = new byte[2];
        using (var fs = File.OpenRead(tempPath))
        {
            if (fs.Read(header, 0, 2) < 2 || header[0] != 0x4D || header[1] != 0x5A)
            {
                try { File.Delete(tempPath); } catch { }
                throw new InvalidOperationException("Скачанный файл поврежден или не является исполняемым (.exe).");
            }
        }

        var currentExe = Environment.ProcessPath ?? Process.GetCurrentProcess().MainModule?.FileName;
        if (string.IsNullOrEmpty(currentExe) || !File.Exists(currentExe))
        {
            throw new InvalidOperationException("Не удалось определить путь к текущему процессу.");
        }

        var oldExe = currentExe + ".old";
        try
        {
            if (File.Exists(oldExe)) File.Delete(oldExe);
            File.Move(currentExe, oldExe);
            File.Move(tempPath, currentExe);

            Process.Start(new ProcessStartInfo(currentExe) { UseShellExecute = true });
            Environment.Exit(0);
        }
        catch
        {
            // Fallback: spawn independent cmd.exe script to wait for exit, overwrite and restart
            var pid = Environment.ProcessId;
            var batchPath = Path.Combine(Path.GetTempPath(), $"update_{pid}.cmd");
            var batchContent = $@"@echo off
timeout /t 1 /nobreak >nul
:waitloop
tasklist /fi ""PID eq {pid}"" | find ""{pid}"" >nul
if not errorlevel 1 (
    timeout /t 1 /nobreak >nul
    goto waitloop
)
move /y ""{tempPath}"" ""{currentExe}"" >nul
start """" ""{currentExe}""
del ""%~f0""
";
            File.WriteAllText(batchPath, batchContent);
            Process.Start(new ProcessStartInfo("cmd.exe", $"/c \"{batchPath}\"")
            {
                CreateNoWindow = true,
                UseShellExecute = false
            });
            Environment.Exit(0);
        }
    }

    public static Version ParseNorm(string v)
    {
        var clean = CleanVersion(v);
        var parts = clean.Split('.');
        int major = parts.Length > 0 && int.TryParse(parts[0], out var ma) ? ma : 0;
        int minor = parts.Length > 1 && int.TryParse(parts[1], out var mi) ? mi : 0;
        int build = parts.Length > 2 && int.TryParse(parts[2], out var bu) ? bu : 0;
        int rev = parts.Length > 3 && int.TryParse(parts[3], out var re) ? re : 0;
        return new Version(major, minor, build, rev);
    }

    public static string CleanVersion(string v)
    {
        v = (v ?? "").Trim();
        if (v.StartsWith("v", StringComparison.OrdinalIgnoreCase)) v = v[1..];
        var dash = v.IndexOf('-');
        if (dash >= 0) v = v[..dash];
        return v.Trim();
    }
}
