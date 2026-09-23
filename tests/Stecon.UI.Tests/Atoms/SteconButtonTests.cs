using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Stecon.UI.Tests;

public class SteconButtonTests : BunitContext
{
    [Fact]
    public void RendersChildContent_AndNativeButtonType()
    {
        var cut = Render<SteconButton>(p => p.AddChildContent("Save"));

        var button = cut.Find("button");
        Assert.Equal("button", button.GetAttribute("type"));
        Assert.Contains("Save", button.TextContent);
    }

    [Theory]
    [InlineData(ButtonType.Button, "button")]
    [InlineData(ButtonType.Submit, "submit")]
    [InlineData(ButtonType.Reset, "reset")]
    public void RendersRequestedNativeType(ButtonType type, string expected)
    {
        var cut = Render<SteconButton>(p => p
            .Add(x => x.Type, type)
            .AddChildContent("Go"));

        Assert.Equal(expected, cut.Find("button").GetAttribute("type"));
    }

    [Fact]
    public void Disabled_PreventsClick()
    {
        var clicked = false;
        var cut = Render<SteconButton>(p => p
            .Add(x => x.Disabled, true)
            .Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, () => clicked = true))
            .AddChildContent("Save"));

        var button = cut.Find("button");
        Assert.True(button.HasAttribute("disabled"));
        button.Click();

        Assert.False(clicked);
    }

    [Fact]
    public void Loading_PreventsClick_AndSetsAriaBusy()
    {
        var clicked = false;
        var cut = Render<SteconButton>(p => p
            .Add(x => x.Loading, true)
            .Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, () => clicked = true))
            .AddChildContent("Save"));

        var button = cut.Find("button");
        Assert.True(button.HasAttribute("disabled"));
        Assert.Equal("true", button.GetAttribute("aria-busy"));
        button.Click();

        Assert.False(clicked);
    }

    [Fact]
    public void Loading_DoesNotHideLabel()
    {
        var cut = Render<SteconButton>(p => p
            .Add(x => x.Loading, true)
            .AddChildContent("Save"));

        Assert.Contains("Save", cut.Find(".stecon-button__label").TextContent);
    }

    [Fact]
    public void EnabledButton_InvokesOnClick()
    {
        var clicked = false;
        var cut = Render<SteconButton>(p => p
            .Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, () => clicked = true))
            .AddChildContent("Save"));

        cut.Find("button").Click();

        Assert.True(clicked);
    }

    [Fact]
    public void RendersIconStartAndIconEnd()
    {
        var cut = Render<SteconButton>(p => p
            .Add(x => x.IconStart, (RenderFragment)(builder => builder.AddContent(0, "S")))
            .Add(x => x.IconEnd, (RenderFragment)(builder => builder.AddContent(0, "E")))
            .AddChildContent("Save"));

        var icons = cut.FindAll(".stecon-button__icon");
        Assert.Equal(2, icons.Count);
        Assert.Equal("S", icons[0].TextContent);
        Assert.Equal("E", icons[1].TextContent);
    }

    [Fact]
    public void ConsumerSuppliedClass_IsMergedWithInternalClasses_AndOtherAttributesPreserved()
    {
        var attributes = new Dictionary<string, object>
        {
            ["class"] = "my-custom-class",
            ["data-testid"] = "save-button",
        };

        var cut = Render<SteconButton>(p => p
            .Add(x => x.AdditionalAttributes, attributes)
            .AddChildContent("Save"));

        var button = cut.Find("button");
        Assert.Contains("stecon-button", button.ClassList);
        Assert.Contains("stecon-button--primary", button.ClassList);
        Assert.Contains("my-custom-class", button.ClassList);
        Assert.Equal("save-button", button.GetAttribute("data-testid"));
        Assert.Equal(1, OpeningTag(button).Split("class=\"").Length - 1);

        // Caller's dictionary must not be mutated.
        Assert.True(attributes.ContainsKey("class"));
        Assert.Equal(2, attributes.Count);
    }

    private static string OpeningTag(AngleSharp.Dom.IElement element) =>
        element.OuterHtml[..(element.OuterHtml.IndexOf('>') + 1)];
}
