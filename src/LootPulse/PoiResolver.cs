using System;

namespace LootPulse;

public static class PoiResolver
{
    private readonly record struct Poi(string Name, float X, float Y, float Radius);

    private static readonly Poi[] Pois =
    [
        // Crossbones Isle (Treasure Hunters / One Piece) — named POIs
        new("Arlong Park", -51707f, 16134f, 20000f),
        new("The Chest", -32433f, -21391f, 20000f),
        new("Rubber Resort", -24028f, -45216f, 20000f),
        new("Buggy's Beachhead", 8270f, -25088f, 20000f),
        new("Lantern Landing", -11770f, 4994f, 18000f),
        new("Windmill Village", -33592f, 63719f, 20000f),
        new("Loguetown", 1846f, 55330f, 20000f),
        new("Plunder Port", 26307f, 89235f, 20000f),
        new("Busy Bay", 62269f, 70367f, 18000f),
        new("The $.$. Rocky", 66471f, 30308f, 20000f),
        new("Cursed Cavern", 31432f, 19822f, 18000f),
        // Landmarks
        new("Baratie", 65771f, 69662f, 10000f),
        new("Nami's Tangerine Grove", -40304f, 82390f, 12000f),
        new("Henry", 34307f, 73225f, 12000f),
        new("Giant's Battlefield", -3614f, 34262f, 14000f),
        new("The Salty Skull", 34752f, 31434f, 12000f)
    ];

    public static string Resolve(float x, float y)
    {
        string bestName = "Окрестности";
        float bestDistSq = float.MaxValue;

        foreach (var p in Pois)
        {
            float dx = x - p.X;
            float dy = y - p.Y;
            float dSq = dx * dx + dy * dy;
            if (dSq <= p.Radius * p.Radius && dSq < bestDistSq)
            {
                bestDistSq = dSq;
                bestName = p.Name;
            }
        }
        return bestName;
    }
}
