using System.Globalization;

namespace Stecon.UI.Tests;

/// <summary>
/// Small, dependency-free WCAG 2.x relative-luminance contrast helper - test-only, not shipped
/// in Stecon.UI. Used to validate M3 palette token pairs deterministically instead of relying on
/// visual judgment (per M3 acceptance criteria).
/// </summary>
internal static class ContrastCalculator
{
    public static double Ratio(string hexA, string hexB)
    {
        var la = RelativeLuminance(hexA);
        var lb = RelativeLuminance(hexB);
        var lighter = Math.Max(la, lb);
        var darker = Math.Min(la, lb);
        return (lighter + 0.05) / (darker + 0.05);
    }

    private static double RelativeLuminance(string hex)
    {
        var (r, g, b) = ParseHex(hex);
        var rl = Linearize(r);
        var gl = Linearize(g);
        var bl = Linearize(b);
        return (0.2126 * rl) + (0.7152 * gl) + (0.0722 * bl);
    }

    private static double Linearize(double channel) =>
        channel <= 0.03928 ? channel / 12.92 : Math.Pow((channel + 0.055) / 1.055, 2.4);

    private static (double R, double G, double B) ParseHex(string hex)
    {
        var value = hex.TrimStart('#');
        var r = int.Parse(value[..2], NumberStyles.HexNumber, CultureInfo.InvariantCulture) / 255.0;
        var g = int.Parse(value.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture) / 255.0;
        var b = int.Parse(value.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture) / 255.0;
        return (r, g, b);
    }
}
