namespace Stecon.UI;

/// <summary>
/// In-memory <see cref="IThemeService"/> implementation. Persistence is
/// intentionally out of scope for M1A (docs/08-THEME-SPEC.md); this
/// contract can be swapped for a persisting implementation later without
/// changing the public API.
/// </summary>
public sealed class ThemeService : IThemeService
{
    private ThemeMode _current = ThemeMode.Light;
    private SteconPalette _currentPalette = SteconPalette.Stecon;

    public ThemeMode Current => _current;

    public event Action<ThemeMode>? ThemeChanged;

    public void SetTheme(ThemeMode mode)
    {
        if (_current == mode)
        {
            return;
        }

        _current = mode;
        ThemeChanged?.Invoke(_current);
    }

    public SteconPalette CurrentPalette => _currentPalette;

    public event Action<SteconPalette>? PaletteChanged;

    public void SetPalette(SteconPalette palette)
    {
        if (_currentPalette == palette)
        {
            return;
        }

        _currentPalette = palette;
        PaletteChanged?.Invoke(_currentPalette);
    }
}
