namespace Stecon.UI;

/// <summary>
/// Accent/personality palette, independent of <see cref="ThemeMode"/> (Light/Dark). Default is
/// <see cref="Stecon"/> - the existing STECON brand-aligned accent, unchanged from before M3.
/// Palette names are direction words derived from visual references, not official STECON brand
/// color specifications - see docs/12-THEME-PALETTE.md.
/// </summary>
public enum SteconPalette
{
    Stecon,
    Romantic,
    Evening,
    Sunset,
    Purple,
    Peach,
    Peacock,
}
