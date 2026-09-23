using Bunit;

namespace Stecon.UI.Tests;

public class SteconCheckboxTests : BunitContext
{
    [Fact]
    public void SupportsTwoWayBinding()
    {
        var value = false;
        var cut = Render<SteconCheckbox>(p => p
            .Add(x => x.Value, value)
            .Add(x => x.ValueChanged, v => value = v)
            .AddChildContent("Accept"));

        cut.Find("input").Change(true);

        Assert.True(value);
    }

    [Fact]
    public void LabelText_IsAssociatedWithInput_ViaImplicitWrapping()
    {
        var cut = Render<SteconCheckbox>(p => p.AddChildContent("Accept terms"));

        var label = cut.Find("label.stecon-checkbox");
        Assert.NotNull(label.QuerySelector("input[type=checkbox]"));
        Assert.Contains("Accept terms", label.TextContent);
    }

    [Fact]
    public void RendersDisabledAttribute()
    {
        var cut = Render<SteconCheckbox>(p => p.Add(x => x.Disabled, true));

        Assert.True(cut.Find("input").HasAttribute("disabled"));
    }

    [Fact]
    public void ConsumerSuppliedClass_IsMergedWithInternalClasses()
    {
        var attributes = new Dictionary<string, object>
        {
            ["class"] = "my-custom-class",
            ["data-testid"] = "accept-checkbox",
        };

        var cut = Render<SteconCheckbox>(p => p.Add(x => x.AdditionalAttributes, attributes));

        var input = cut.Find("input");
        Assert.Contains("stecon-checkbox__input", input.ClassList);
        Assert.Contains("my-custom-class", input.ClassList);
        Assert.Equal("accept-checkbox", input.GetAttribute("data-testid"));
        Assert.Equal(1, input.OuterHtml.Split("class=\"").Length - 1);
        Assert.Equal(2, attributes.Count);
    }
}
