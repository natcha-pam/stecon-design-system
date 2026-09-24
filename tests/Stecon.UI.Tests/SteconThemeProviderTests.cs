using Bunit;
using Microsoft.Extensions.DependencyInjection;

namespace Stecon.UI.Tests;

public class SteconThemeProviderTests : BunitContext
{
    public SteconThemeProviderTests()
    {
        Services.AddSteconTheme();
    }

    [Fact]
    public void RendersLightThemeAttribute_ByDefault()
    {
        var cut = Render<SteconThemeProvider>(parameters => parameters
            .AddChildContent("<p>content</p>"));

        Assert.Equal("light", cut.Find("[data-theme]").GetAttribute("data-theme"));
    }

    [Fact]
    public void RendersRequestedInitialTheme()
    {
        var cut = Render<SteconThemeProvider>(parameters => parameters
            .Add(p => p.InitialTheme, ThemeMode.Dark)
            .AddChildContent("<p>content</p>"));

        Assert.Equal("dark", cut.Find("[data-theme]").GetAttribute("data-theme"));
    }

    [Fact]
    public void UpdatesThemeAttribute_WhenThemeServiceChanges()
    {
        var cut = Render<SteconThemeProvider>(parameters => parameters
            .AddChildContent("<p>content</p>"));
        var themeService = Services.GetRequiredService<IThemeService>();

        themeService.SetTheme(ThemeMode.Dark);

        Assert.Equal("dark", cut.Find("[data-theme]").GetAttribute("data-theme"));
    }

    [Fact]
    public void RendersSteconPaletteAttribute_ByDefault()
    {
        var cut = Render<SteconThemeProvider>(parameters => parameters
            .AddChildContent("<p>content</p>"));

        Assert.Equal("stecon", cut.Find("[data-palette]").GetAttribute("data-palette"));
    }

    [Theory]
    [InlineData(SteconPalette.Romantic, "romantic")]
    [InlineData(SteconPalette.Evening, "evening")]
    [InlineData(SteconPalette.Sunset, "sunset")]
    [InlineData(SteconPalette.Purple, "purple")]
    [InlineData(SteconPalette.Peach, "peach")]
    [InlineData(SteconPalette.Peacock, "peacock")]
    public void RendersRequestedInitialPalette(SteconPalette palette, string expectedMarker)
    {
        var cut = Render<SteconThemeProvider>(parameters => parameters
            .Add(p => p.InitialPalette, palette)
            .AddChildContent("<p>content</p>"));

        Assert.Equal(expectedMarker, cut.Find("[data-palette]").GetAttribute("data-palette"));
    }

    [Fact]
    public void UpdatesPaletteAttribute_WhenThemeServiceChanges()
    {
        var cut = Render<SteconThemeProvider>(parameters => parameters
            .AddChildContent("<p>content</p>"));
        var themeService = Services.GetRequiredService<IThemeService>();

        themeService.SetPalette(SteconPalette.Peacock);

        Assert.Equal("peacock", cut.Find("[data-palette]").GetAttribute("data-palette"));
    }

    [Fact]
    public void ModeChange_DoesNotAffectPaletteAttribute_AndViceVersa()
    {
        var cut = Render<SteconThemeProvider>(parameters => parameters
            .Add(p => p.InitialPalette, SteconPalette.Purple)
            .AddChildContent("<p>content</p>"));
        var themeService = Services.GetRequiredService<IThemeService>();

        themeService.SetTheme(ThemeMode.Dark);
        Assert.Equal("purple", cut.Find("[data-palette]").GetAttribute("data-palette"));

        themeService.SetPalette(SteconPalette.Sunset);
        Assert.Equal("dark", cut.Find("[data-theme]").GetAttribute("data-theme"));
    }
}
