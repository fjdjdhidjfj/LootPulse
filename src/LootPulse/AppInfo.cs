using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LootPulse;

public static class AppInfo
{
    public const string Name = "LootPulse";
    public const string Version = "2.1.1";
    public const string Tagline = "Fortnite Chest & Squad Tracker";

    public static string DefaultDataDirectory
    {
        get
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var pulseDir = Path.Combine(appData, Name);

            // Seamless migration from legacy ChestCounter if present
            if (!Directory.Exists(pulseDir))
            {
                var legacyDir = Path.Combine(appData, "ChestCounter");
                if (Directory.Exists(legacyDir))
                {
                    try
                    {
                        Directory.CreateDirectory(pulseDir);
                        foreach (var file in Directory.GetFiles(legacyDir))
                        {
                            var dest = Path.Combine(pulseDir, Path.GetFileName(file));
                            if (!File.Exists(dest)) File.Copy(file, dest, overwrite: false);
                        }
                    }
                    catch
                    {
                        // Ignore migration error, fallback to clean directory
                    }
                }
            }

            Directory.CreateDirectory(pulseDir);
            return pulseDir;
        }
    }

    public static string InstanceMutexName(string dir)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(Path.GetFullPath(dir).ToLowerInvariant()));
        return @"Local\LootPulse.Instance." + Convert.ToHexString(hash, 0, 8);
    }

    public static string ShowEventName(string dir) => InstanceMutexName(dir).Replace(".Instance.", ".Show.");
}
