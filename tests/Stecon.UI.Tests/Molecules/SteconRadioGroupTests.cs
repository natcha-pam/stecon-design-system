using Bunit;

namespace Stecon.UI.Tests;

public class SteconRadioGroupTests : BunitContext
{
    [Fact]
    public void SupportsTwoWayBinding_AndExclusiveSelection()
    {
        var selected = "a";
        var cut = Render<SteconRadioGroup<string>>(p => p
            .Add(x => x.Value, selected)
            .Add(x => x.ValueChanged, v => selected = v)
            .Add(x => x.Label, "Choice")
            .AddChildContent(builder =>
            {
                builder.OpenComponent<SteconRadio<string>>(0);
                builder.AddComponentParameter(1, "Value", "a");
                builder.AddComponentParameter(2, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b => b.AddContent(0, "A")));
                builder.CloseComponent();

                builder.OpenComponent<SteconRadio<string>>(3);
                builder.AddComponentParameter(4, "Value", "b");
                builder.AddComponentParameter(5, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b => b.AddContent(0, "B")));
                builder.CloseComponent();
            }));

        var radios = cut.FindAll("input[type=radio]");
        Assert.True(radios[0].HasAttribute("checked"));
        Assert.False(radios[1].HasAttribute("checked"));

        radios[1].Change(true);

        Assert.Equal("b", selected);
    }

    [Fact]
    public void RendersFieldsetLegend_WhenLabelProvided()
    {
        var cut = Render<SteconRadioGroup<string>>(p => p
            .Add(x => x.Value, "a")
            .Add(x => x.Label, "Shipping")
            .AddChildContent("<p>content</p>"));

        var fieldset = cut.Find("fieldset.stecon-radio-group");
        Assert.Equal("Shipping", fieldset.QuerySelector("legend")!.TextContent);
        Assert.False(fieldset.HasAttribute("role"));
    }

    [Fact]
    public void RendersRadiogroupRole_WhenNoLabelProvided()
    {
        var cut = Render<SteconRadioGroup<string>>(p => p
            .Add(x => x.Value, "a")
            .AddChildContent("<p>content</p>"));

        var div = cut.Find("div.stecon-radio-group");
        Assert.Equal("radiogroup", div.GetAttribute("role"));
    }

    [Fact]
    public void TwoIndependentGroups_GenerateNonCollidingNames()
    {
        var groupA = Render<SteconRadioGroup<string>>(p => p
            .Add(x => x.Value, "a")
            .AddChildContent(builder =>
            {
                builder.OpenComponent<SteconRadio<string>>(0);
                builder.AddComponentParameter(1, "Value", "a");
                builder.CloseComponent();
            }));

        var groupB = Render<SteconRadioGroup<string>>(p => p
            .Add(x => x.Value, "a")
            .AddChildContent(builder =>
            {
                builder.OpenComponent<SteconRadio<string>>(0);
                builder.AddComponentParameter(1, "Value", "a");
                builder.CloseComponent();
            }));

        var nameA = groupA.Find("input[type=radio]").GetAttribute("name");
        var nameB = groupB.Find("input[type=radio]").GetAttribute("name");

        Assert.NotNull(nameA);
        Assert.NotNull(nameB);
        Assert.NotEqual(nameA, nameB);
    }
}
