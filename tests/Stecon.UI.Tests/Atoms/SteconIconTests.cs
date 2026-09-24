using Bunit;

namespace Stecon.UI.Tests;

public class SteconIconTests : BunitContext
{
    [Fact]
    public void RendersSvg_WithAriaHidden_WhenNoTitle()
    {
        var cut = Render<SteconIcon>(p => p.Add(x => x.Name, IconName.Home));

        var svg = cut.Find("svg.stecon-icon");
        Assert.Equal("true", svg.GetAttribute("aria-hidden"));
        Assert.Empty(cut.FindAll("title"));
    }

    [Fact]
    public void RendersAccessibleTitle_WhenSupplied()
    {
        var cut = Render<SteconIcon>(p => p
            .Add(x => x.Name, IconName.Menu)
            .Add(x => x.Title, "Open navigation"));

        var svg = cut.Find("svg.stecon-icon");
        Assert.Equal("img", svg.GetAttribute("role"));
        Assert.Equal("Open navigation", cut.Find("title").TextContent);
    }

    [Theory]
    [InlineData(IconName.InfoCircle)]
    [InlineData(IconName.CheckCircle)]
    [InlineData(IconName.ExclamationTriangle)]
    [InlineData(IconName.XCircle)]
    public void RendersNewStatusIcons_WithoutError(IconName name)
    {
        var cut = Render<SteconIcon>(p => p.Add(x => x.Name, name));

        var svg = cut.Find("svg.stecon-icon");
        Assert.NotEmpty(svg.Children);
    }
}
