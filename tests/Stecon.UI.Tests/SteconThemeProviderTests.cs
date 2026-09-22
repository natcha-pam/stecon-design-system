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
}
