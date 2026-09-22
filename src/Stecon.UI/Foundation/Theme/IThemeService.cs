namespace Stecon.UI;

/// <summary>
/// Holds the current STECON UI theme and notifies subscribers on change.
/// See docs/08-THEME-SPEC.md - consumers should go through <see cref="SteconThemeProvider"/>
/// rather than setting DOM attributes directly.
/// </summary>
public interface IThemeService
{
    ThemeMode Current { get; }

    event Action<ThemeMode>? ThemeChanged;

    void SetTheme(ThemeMode mode);
}
