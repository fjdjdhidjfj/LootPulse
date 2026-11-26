using System;
using System.Collections.Generic;

namespace LootPulse;

public static class ChestClassifier
{
    public static readonly string[] Order = ["normal", "rare", "weapon", "buried", "ghost"];

    public static string Classify(string? label)
    {
        if (string.IsNullOrWhiteSpace(label)) return "normal";

        if (ContainsAny(label, "Ghost", "Spirit", "Phantom", "Shadow", "Soul", "Spooky", "Haunted", "Henchman"))
            return "ghost";

        if (ContainsAny(label, "BuriedChest", "Treasure"))
            return "buried";

        if (ContainsAny(label, "RareChest", "Tiered_Chest_6", "EpicChest", "LegendaryChest", "GodChest"))
            return "rare";

        if (ContainsAny(label, "Relics", "Delilah", "Weapon", "Faction", "Heist", "SupplyDrop", "Doom", "Avenger", "Oathbound"))
            return "weapon";

        return "normal";
    }

    public static Dictionary<string, int> Summarize(IEnumerable<ChestEvent> events)
    {
        var counts = new Dictionary<string, int>();
        foreach (var ev in events)
        {
            var key = Classify(ev.Label);
            counts[key] = counts.GetValueOrDefault(key, 0) + 1;
        }

        var ordered = new Dictionary<string, int>();
        foreach (var k in Order)
        {
            if (counts.TryGetValue(k, out var c) && c > 0)
            {
                ordered[k] = c;
            }
        }
        return ordered;
    }

    private static bool ContainsAny(string text, params string[] needles)
    {
        foreach (var n in needles)
        {
            if (text.Contains(n, StringComparison.OrdinalIgnoreCase)) return true;
        }
        return false;
    }
}
