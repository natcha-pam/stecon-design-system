namespace Stecon.UI.Tests;

/// <summary>
/// Deterministic WCAG contrast validation for every Mode x Palette accent used by M3 (see
/// docs/12-THEME-PALETTE.md). Validates the token pairs actually used in CSS: solid accent fills
/// against on-interactive text (buttons), and accent-soft surfaces against the accent text color
/// (selected nav/rows). Does not test pixel-perfect colors - only contrast ratios.
/// </summary>
public class PaletteContrastTests
{
    private sealed record PaletteColors(string Light, string LightHover, string LightActive, string LightSoft, string Dark, string DarkHover, string DarkActive, string DarkSoft, string DarkOnSoft);

    // Dark-mode Hover/Active reuse the Light-mode Hover/Active values (same rationale as Default
    // reusing Light's accent) - the simplest, smallest way to guarantee >=4.5:1 in both Modes,
    // since Light's hover/active shades are already darker/more saturated than Light's own
    // accent (itself already verified >=4.5:1).
    private static readonly Dictionary<string, PaletteColors> Palettes = new()
    {
        ["Stecon"] = new("#1d4ed8", "#1739ad", "#142d85", "#e8edfc", "#3f63d1", "#1739ad", "#2f4bb0", "#1c2947", "#4c9be0"),
        ["Romantic"] = new("#be185d", "#9d174d", "#831843", "#fce7f3", "#be185d", "#9d174d", "#831843", "#2a0f1c", "#f472b6"),
        ["Evening"] = new("#4c3a75", "#3e2f60", "#31254c", "#ece8f5", "#4c3a75", "#3e2f60", "#31254c", "#1a1530", "#b7a9e6"),
        ["Sunset"] = new("#c2410c", "#9a3412", "#7c2d12", "#ffedd5", "#c2410c", "#9a3412", "#7c2d12", "#2b1206", "#fb923c"),
        ["Purple"] = new("#7c3aed", "#6d28d9", "#5b21b6", "#f1e8fd", "#7c3aed", "#6d28d9", "#5b21b6", "#1f1440", "#c4b5fd"),
        ["Peach"] = new("#a34a1f", "#8a3e1a", "#702f14", "#fdece0", "#a34a1f", "#8a3e1a", "#702f14", "#301708", "#f0a37a"),
        ["Peacock"] = new("#0e7490", "#155e75", "#164e63", "#e0f2fe", "#0e7490", "#155e75", "#164e63", "#0a2530", "#38bdf8"),
    };

    private const string White = "#ffffff";

    public static IEnumerable<object[]> AllPalettes() => Palettes.Keys.Select(k => new object[] { k });

    [Theory]
    [MemberData(nameof(AllPalettes))]
    public void LightAccent_MeetsAA_AgainstWhiteOnInteractiveText(string palette)
    {
        var c = Palettes[palette];
        var ratio = ContrastCalculator.Ratio(c.Light, White);
        Assert.True(ratio >= 4.5, $"{palette} light accent {c.Light} vs white = {ratio:0.00}:1 (need >=4.5:1)");
    }

    // Hover/Active must meet the same WCAG AA normal-text threshold (4.5:1) as the resting
    // accent - the large-text exception (3:1) is no longer used for interactive button fills.
    [Theory]
    [MemberData(nameof(AllPalettes))]
    public void LightHover_MeetsAA_AgainstWhiteOnInteractiveText(string palette)
    {
        var c = Palettes[palette];
        var ratio = ContrastCalculator.Ratio(c.LightHover, White);
        Assert.True(ratio >= 4.5, $"{palette} light hover {c.LightHover} vs white = {ratio:0.00}:1 (need >=4.5:1)");
    }

    [Theory]
    [MemberData(nameof(AllPalettes))]
    public void LightActive_MeetsAA_AgainstWhiteOnInteractiveText(string palette)
    {
        var c = Palettes[palette];
        var ratio = ContrastCalculator.Ratio(c.LightActive, White);
        Assert.True(ratio >= 4.5, $"{palette} light active {c.LightActive} vs white = {ratio:0.00}:1 (need >=4.5:1)");
    }

    [Theory]
    [MemberData(nameof(AllPalettes))]
    public void DarkAccent_MeetsAA_AgainstWhiteOnInteractiveText(string palette)
    {
        var c = Palettes[palette];
        var ratio = ContrastCalculator.Ratio(c.Dark, White);
        Assert.True(ratio >= 4.5, $"{palette} dark accent {c.Dark} vs white = {ratio:0.00}:1 (need >=4.5:1)");
    }

    [Theory]
    [MemberData(nameof(AllPalettes))]
    public void DarkHover_MeetsAA_AgainstWhiteOnInteractiveText(string palette)
    {
        var c = Palettes[palette];
        var ratio = ContrastCalculator.Ratio(c.DarkHover, White);
        Assert.True(ratio >= 4.5, $"{palette} dark hover {c.DarkHover} vs white = {ratio:0.00}:1 (need >=4.5:1)");
    }

    [Theory]
    [MemberData(nameof(AllPalettes))]
    public void DarkActive_MeetsAA_AgainstWhiteOnInteractiveText(string palette)
    {
        var c = Palettes[palette];
        var ratio = ContrastCalculator.Ratio(c.DarkActive, White);
        Assert.True(ratio >= 4.5, $"{palette} dark active {c.DarkActive} vs white = {ratio:0.00}:1 (need >=4.5:1)");
    }

    [Theory]
    [MemberData(nameof(AllPalettes))]
    public void LightAccentText_MeetsAA_OnLightSoftSurface(string palette)
    {
        var c = Palettes[palette];
        var ratio = ContrastCalculator.Ratio(c.Light, c.LightSoft);
        Assert.True(ratio >= 4.5, $"{palette} light accent text {c.Light} on soft {c.LightSoft} = {ratio:0.00}:1 (need >=4.5:1)");
    }

    // In dark mode, text drawn on the tinted -soft surface uses a lighter tint of the same hue
    // than the solid-fill accent (which must stay dark/saturated for white on-interactive text) -
    // two different luminance needs for the same hue, exactly like the pre-existing
    // status-info/status-info-surface pair. Found via this contrast test, not assumed.
    [Theory]
    [MemberData(nameof(AllPalettes))]
    public void DarkAccentOnSoftText_MeetsAA_OnDarkSoftSurface(string palette)
    {
        var c = Palettes[palette];
        var ratio = ContrastCalculator.Ratio(c.DarkOnSoft, c.DarkSoft);
        Assert.True(ratio >= 4.5, $"{palette} dark accent-on-soft text {c.DarkOnSoft} on soft {c.DarkSoft} = {ratio:0.00}:1 (need >=4.5:1)");
    }
}
