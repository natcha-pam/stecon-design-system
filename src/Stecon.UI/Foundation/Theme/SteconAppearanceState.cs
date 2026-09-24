namespace Stecon.UI;

/// <summary>
/// Serializable appearance ("Mode x Palette") state, independent of business data and of
/// <see cref="SteconTableLayoutState"/> (table personalization). Plain enums only, so it
/// round-trips through <c>System.Text.Json</c> with no custom converters. Persistence
/// (localStorage/database) is intentionally out of scope for M3.
/// </summary>
public sealed record SteconAppearanceState(ThemeMode Mode, SteconPalette Palette);
