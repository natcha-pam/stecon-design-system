using Bunit;

namespace Stecon.UI.Tests;

public class SteconSelectTests : BunitContext
{
    [Fact]
    public void SupportsTwoWayBinding_ForStringValues()
    {
        var value = "a";
        var cut = Render<SteconSelect<string>>(p => p
            .Add(x => x.Value, value)
            .Add(x => x.ValueChanged, v => value = v)
            .AddChildContent("<option value=\"a\">A</option><option value=\"b\">B</option>"));

        cut.Find("select").Change("b");

        Assert.Equal("b", value);
    }

    [Fact]
    public void SupportsGenericEnumTValue()
    {
        var value = BadgeVariant.Neutral;
        var cut = Render<SteconSelect<BadgeVariant>>(p => p
            .Add(x => x.Value, value)
            .Add(x => x.ValueChanged, v => value = v)
            .AddChildContent(builder =>
            {
                foreach (var variant in Enum.GetValues<BadgeVariant>())
                {
                    builder.OpenElement(0, "option");
                    builder.AddAttribute(1, "value", variant.ToString());
                    builder.AddContent(2, variant.ToString());
                    builder.CloseElement();
                }
            }));

        cut.Find("select").Change(nameof(BadgeVariant.Danger));

        Assert.Equal(BadgeVariant.Danger, value);
    }

    [Fact]
    public void SupportsNullableSupportedTValue()
    {
        int? value = 1;
        var cut = Render<SteconSelect<int?>>(p => p
            .Add(x => x.Value, value)
            .Add(x => x.ValueChanged, v => value = v)
            .AddChildContent("<option value=\"\">-- none --</option><option value=\"1\">One</option><option value=\"2\">Two</option>"));

        cut.Find("select").Change("2");
        Assert.Equal(2, value);

        cut.Find("select").Change("");
        Assert.Null(value);
    }

    [Fact]
    public void InvalidConversion_DoesNotThrow_AndLeavesValueUnchanged()
    {
        var value = 1;
        var changedInvoked = false;
        var cut = Render<SteconSelect<int>>(p => p
            .Add(x => x.Value, value)
            .Add(x => x.ValueChanged, v => { value = v; changedInvoked = true; })
            .AddChildContent("<option value=\"1\">One</option><option value=\"2\">Two</option>"));

        var exception = Record.Exception(() => cut.Find("select").Change("not-a-number"));

        Assert.Null(exception);
        Assert.False(changedInvoked);
        Assert.Equal(1, value);
    }

    [Fact]
    public void RendersDisabledAttribute()
    {
        var cut = Render<SteconSelect<string>>(p => p
            .Add(x => x.Disabled, true)
            .AddChildContent("<option value=\"a\">A</option>"));

        Assert.True(cut.Find("select").HasAttribute("disabled"));
    }

    [Fact]
    public void RendersAriaInvalid()
    {
        var cut = Render<SteconSelect<string>>(p => p
            .Add(x => x.IsInvalid, true)
            .AddChildContent("<option value=\"a\">A</option>"));

        Assert.Equal("true", cut.Find("select").GetAttribute("aria-invalid"));
    }

    [Fact]
    public void ConsumerSuppliedClass_IsMergedWithInternalClasses()
    {
        var attributes = new Dictionary<string, object>
        {
            ["class"] = "my-custom-class",
            ["data-testid"] = "variant-select",
        };

        var cut = Render<SteconSelect<string>>(p => p
            .Add(x => x.AdditionalAttributes, attributes)
            .AddChildContent("<option value=\"a\">A</option>"));

        var select = cut.Find("select");
        Assert.Contains("stecon-select", select.ClassList);
        Assert.Contains("my-custom-class", select.ClassList);
        Assert.Equal("variant-select", select.GetAttribute("data-testid"));
        Assert.Equal(1, select.OuterHtml.Split("class=\"").Length - 1);
        Assert.Equal(2, attributes.Count);
    }
}
