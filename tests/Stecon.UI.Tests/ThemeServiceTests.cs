namespace Stecon.UI.Tests;

public class ThemeServiceTests
{
    [Fact]
    public void Current_DefaultsToLight()
    {
        var sut = new ThemeService();

        Assert.Equal(ThemeMode.Light, sut.Current);
    }

    [Fact]
    public void SetTheme_UpdatesCurrent()
    {
        var sut = new ThemeService();

        sut.SetTheme(ThemeMode.Dark);

        Assert.Equal(ThemeMode.Dark, sut.Current);
    }

    [Fact]
    public void SetTheme_RaisesThemeChanged_WhenValueChanges()
    {
        var sut = new ThemeService();
        ThemeMode? raised = null;
        sut.ThemeChanged += mode => raised = mode;

        sut.SetTheme(ThemeMode.Dark);

        Assert.Equal(ThemeMode.Dark, raised);
    }

    [Fact]
    public void SetTheme_DoesNotRaiseThemeChanged_WhenValueIsUnchanged()
    {
        var sut = new ThemeService();
        var raiseCount = 0;
        sut.ThemeChanged += _ => raiseCount++;

        sut.SetTheme(ThemeMode.Light);

        Assert.Equal(0, raiseCount);
    }

    [Fact]
    public void CurrentPalette_DefaultsToStecon()
    {
        var sut = new ThemeService();

        Assert.Equal(SteconPalette.Stecon, sut.CurrentPalette);
    }

    [Fact]
    public void SetPalette_UpdatesCurrentPalette()
    {
        var sut = new ThemeService();

        sut.SetPalette(SteconPalette.Peacock);

        Assert.Equal(SteconPalette.Peacock, sut.CurrentPalette);
    }

    [Fact]
    public void SetPalette_RaisesPaletteChanged_WhenValueChanges()
    {
        var sut = new ThemeService();
        SteconPalette? raised = null;
        sut.PaletteChanged += p => raised = p;

        sut.SetPalette(SteconPalette.Purple);

        Assert.Equal(SteconPalette.Purple, raised);
    }

    [Fact]
    public void SetPalette_DoesNotRaisePaletteChanged_WhenValueIsUnchanged()
    {
        var sut = new ThemeService();
        var raiseCount = 0;
        sut.PaletteChanged += _ => raiseCount++;

        sut.SetPalette(SteconPalette.Stecon);

        Assert.Equal(0, raiseCount);
    }

    [Fact]
    public void SetPalette_DoesNotAffectMode_AndSetTheme_DoesNotAffectPalette()
    {
        var sut = new ThemeService();

        sut.SetPalette(SteconPalette.Sunset);
        Assert.Equal(ThemeMode.Light, sut.Current);

        sut.SetTheme(ThemeMode.Dark);
        Assert.Equal(SteconPalette.Sunset, sut.CurrentPalette);
    }
}
