namespace Stecon.UI;

/// <summary>
/// All user-facing text for <see cref="SteconAppearanceSettings"/>. No English/Thai text is
/// invented inside the library - every label is required, mirroring the M2B
/// <c>SteconDataTableSettingsLabels</c> precedent. Enum identifiers (e.g. <see cref="SteconPalette"/>
/// members) stay English code identifiers; only the visible label text is caller-supplied.
/// </summary>
public sealed record SteconAppearanceSettingsLabels(
    string ModeSectionLabel,
    string LightLabel,
    string DarkLabel,
    string PaletteSectionLabel,
    string SteconPaletteLabel,
    string RomanticPaletteLabel,
    string EveningPaletteLabel,
    string SunsetPaletteLabel,
    string PurplePaletteLabel,
    string PeachPaletteLabel,
    string PeacockPaletteLabel,
    string ResetLabel);
