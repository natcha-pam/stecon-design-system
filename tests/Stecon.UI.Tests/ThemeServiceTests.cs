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
}
