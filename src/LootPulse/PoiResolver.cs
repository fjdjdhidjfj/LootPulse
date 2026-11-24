using System;

namespace LootPulse;

public static class PoiResolver
{
    private readonly record struct Poi(string Name, float X, float Y, float Radius);

    private static readonly Poi[] KnownPois =
    [
        new("The Agency", 0f, -5000f, 25000f),
        new("Salty Springs", -26000f, -25000f, 20000f),
        new("Pleasant Park", -25000f, -72000f, 22000f),
        new("Sweaty Sands", -70000f, -52000f, 22000f),
        new("Holly Hedges", -75000f, -10000f, 20000f),
        new("Weeping Woods", -22000f, -44000f, 24000f),
        new("Slurpy Swamp", -40000f, 18000f, 24000f),
        new("Misty Meadows", 15000f, 75000f, 22000f),
        new("Lazy Lake", 20000f, 30000f, 22000f),
        new("Retail Row", 64000f, 28000f, 22000f),
        new("Dirty Docks", 85000f, -25000f, 24000f),
        new("Frenzy Farm", 25000f, -22000f, 22000f),
        new("Steamy Stacks", 68000f, -74000f, 22000f),
        new("Craggy Cliffs", 15000f, -86000f, 20000f),
        new("Catty Corner", 52000f, 66000f, 18000f),
        new("Risky Reels", 12000f, -15000f, 15000f),
        new("Coral Castle", -75000f, -85000f, 22000f),
        new("Doom's Courtyard", -25000f, -68000f, 22000f)
    ];

    public static string Resolve(float x, float y)
    {
        string best = "Окрестности";
        float bestDistSq = float.MaxValue;

        foreach (var p in KnownPois)
        {
            var dx = x - p.X;
            var dy = y - p.Y;
            var dSq = dx * dx + dy * dy;
            if (dSq <= p.Radius * p.Radius && dSq < bestDistSq)
            {
                bestDistSq = dSq;
                best = p.Name;
            }
        }

        return best;
    }
}
