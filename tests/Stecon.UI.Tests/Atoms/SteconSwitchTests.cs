using Bunit;

namespace Stecon.UI.Tests;

public class SteconSwitchTests : BunitContext
{
    [Fact]
    public void SupportsTwoWayBinding()
    {
        var value = false;
        var cut = Render<SteconSwitch>(p => p
            .Add(x => x.Value, value)
            .Add(x => x.ValueChanged, v => value = v));

        cut.Find("input").Change(true);

        Assert.True(value);
    }

    [Fact]
    public void RendersNativeCheckboxWithSwitchRole()
    {
        var cut = Render<SteconSwitch>();

        var input = cut.Find("input");
        Assert.Equal("checkbox", input.GetAttribute("type"));
        Assert.Equal("switch", input.GetAttribute("role"));
    }

    [Fact]
    public void RendersDisabledAttribute()
    {
        var cut = Render<SteconSwitch>(p => p.Add(x => x.Disabled, true));

        Assert.True(cut.Find("input").HasAttribute("disabled"));
    }

    [Fact]
    public void ConsumerSuppliedClass_IsMergedWithInternalClasses()
    {
        var attributes = new Dictionary<string, object>
        {
            ["class"] = "my-custom-class",
            ["data-testid"] = "notifications-switch",
        };

        var cut = Render<SteconSwitch>(p => p.Add(x => x.AdditionalAttributes, attributes));

        var input = cut.Find("input");
        Assert.Contains("stecon-switch__input", input.ClassList);
        Assert.Contains("my-custom-class", input.ClassList);
        Assert.Equal("notifications-switch", input.GetAttribute("data-testid"));
        Assert.Equal(1, input.OuterHtml.Split("class=\"").Length - 1);
        Assert.Equal(2, attributes.Count);
    }
}
