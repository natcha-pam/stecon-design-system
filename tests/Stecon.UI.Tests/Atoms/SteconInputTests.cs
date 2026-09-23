using Bunit;

namespace Stecon.UI.Tests;

public class SteconInputTests : BunitContext
{
    [Fact]
    public void SupportsTwoWayBinding()
    {
        string? value = "initial";
        var cut = Render<SteconInput>(p => p
            .Add(x => x.Value, value)
            .Add(x => x.ValueChanged, v => value = v));

        cut.Find("input").Input("updated");

        Assert.Equal("updated", value);
    }

    [Fact]
    public void RendersNativeDisabledAndReadOnlyAttributes()
    {
        var cut = Render<SteconInput>(p => p
            .Add(x => x.Disabled, true)
            .Add(x => x.ReadOnly, true));

        var input = cut.Find("input");
        Assert.True(input.HasAttribute("disabled"));
        Assert.True(input.HasAttribute("readonly"));
    }

    [Fact]
    public void RendersAriaInvalid_AndAriaDescribedBy()
    {
        var cut = Render<SteconInput>(p => p
            .Add(x => x.IsInvalid, true)
            .Add(x => x.DescribedBy, "error-1"));

        var input = cut.Find("input");
        Assert.Equal("true", input.GetAttribute("aria-invalid"));
        Assert.Equal("error-1", input.GetAttribute("aria-describedby"));
    }

    [Theory]
    [InlineData(TextInputType.Email, "email")]
    [InlineData(TextInputType.Password, "password")]
    [InlineData(TextInputType.Number, "number")]
    public void RendersRequestedNativeInputType(TextInputType type, string expected)
    {
        var cut = Render<SteconInput>(p => p.Add(x => x.Type, type));

        Assert.Equal(expected, cut.Find("input").GetAttribute("type"));
    }

    [Fact]
    public void ConsumerSuppliedClass_IsMergedWithInternalClasses()
    {
        var attributes = new Dictionary<string, object>
        {
            ["class"] = "my-custom-class",
            ["data-testid"] = "name-input",
        };

        var cut = Render<SteconInput>(p => p.Add(x => x.AdditionalAttributes, attributes));

        var input = cut.Find("input");
        Assert.Contains("stecon-input", input.ClassList);
        Assert.Contains("my-custom-class", input.ClassList);
        Assert.Equal("name-input", input.GetAttribute("data-testid"));
        Assert.Equal(1, input.OuterHtml.Split("class=\"").Length - 1);
        Assert.Equal(2, attributes.Count);
    }
}
