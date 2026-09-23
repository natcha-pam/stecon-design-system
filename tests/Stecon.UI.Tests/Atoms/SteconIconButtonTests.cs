using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Stecon.UI.Tests;

public class SteconIconButtonTests : BunitContext
{
    [Fact]
    public void RendersAccessibleLabel_AsAriaLabelAndTitle()
    {
        var cut = Render<SteconIconButton>(p => p
            .Add(x => x.AccessibleLabel, "Close dialog")
            .Add(x => x.Icon, IconName.Close));

        var button = cut.Find("button");
        Assert.Equal("Close dialog", button.GetAttribute("aria-label"));
        Assert.Equal("Close dialog", button.GetAttribute("title"));
    }

    [Fact]
    public void IconContent_TakesPrecedenceOverIcon()
    {
        var cut = Render<SteconIconButton>(p => p
            .Add(x => x.AccessibleLabel, "Custom")
            .Add(x => x.Icon, IconName.Close)
            .Add(x => x.IconContent, (RenderFragment)(builder => builder.AddContent(0, "CUSTOM-ICON"))));

        Assert.Contains("CUSTOM-ICON", cut.Find("button").TextContent);
        Assert.Empty(cut.FindAll("svg"));
    }

    [Fact]
    public void Disabled_PreventsClick()
    {
        var clicked = false;
        var cut = Render<SteconIconButton>(p => p
            .Add(x => x.AccessibleLabel, "Close")
            .Add(x => x.Disabled, true)
            .Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, () => clicked = true)));

        var button = cut.Find("button");
        Assert.True(button.HasAttribute("disabled"));
        button.Click();

        Assert.False(clicked);
    }

    [Fact]
    public void Loading_PreventsClick_AndSetsAriaBusy()
    {
        var clicked = false;
        var cut = Render<SteconIconButton>(p => p
            .Add(x => x.AccessibleLabel, "Close")
            .Add(x => x.Loading, true)
            .Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, () => clicked = true)));

        var button = cut.Find("button");
        Assert.Equal("true", button.GetAttribute("aria-busy"));
        button.Click();

        Assert.False(clicked);
    }

    [Fact]
    public void ConsumerSuppliedClass_IsMergedWithInternalClasses()
    {
        var attributes = new Dictionary<string, object>
        {
            ["class"] = "my-custom-class",
            ["data-testid"] = "close-button",
        };

        var cut = Render<SteconIconButton>(p => p
            .Add(x => x.AccessibleLabel, "Close")
            .Add(x => x.AdditionalAttributes, attributes));

        var button = cut.Find("button");
        Assert.Contains("stecon-icon-button", button.ClassList);
        Assert.Contains("my-custom-class", button.ClassList);
        Assert.Equal("close-button", button.GetAttribute("data-testid"));
        Assert.Equal(1, OpeningTag(button).Split("class=\"").Length - 1);
        Assert.Equal(2, attributes.Count);
    }

    private static string OpeningTag(AngleSharp.Dom.IElement element) =>
        element.OuterHtml[..(element.OuterHtml.IndexOf('>') + 1)];
}
