using Bunit;

namespace Stecon.UI.Tests;

public class SteconTextareaTests : BunitContext
{
    [Fact]
    public void SupportsTwoWayBinding()
    {
        string? value = "initial";
        var cut = Render<SteconTextarea>(p => p
            .Add(x => x.Value, value)
            .Add(x => x.ValueChanged, v => value = v));

        cut.Find("textarea").Input("updated");

        Assert.Equal("updated", value);
    }

    [Fact]
    public void RendersNativeDisabledAndReadOnlyAttributes()
    {
        var cut = Render<SteconTextarea>(p => p
            .Add(x => x.Disabled, true)
            .Add(x => x.ReadOnly, true));

        var textarea = cut.Find("textarea");
        Assert.True(textarea.HasAttribute("disabled"));
        Assert.True(textarea.HasAttribute("readonly"));
    }

    [Fact]
    public void RendersAriaInvalid_AndAriaDescribedBy()
    {
        var cut = Render<SteconTextarea>(p => p
            .Add(x => x.IsInvalid, true)
            .Add(x => x.DescribedBy, "error-1"));

        var textarea = cut.Find("textarea");
        Assert.Equal("true", textarea.GetAttribute("aria-invalid"));
        Assert.Equal("error-1", textarea.GetAttribute("aria-describedby"));
    }

    [Fact]
    public void ConsumerSuppliedClass_IsMergedWithInternalClasses()
    {
        var attributes = new Dictionary<string, object>
        {
            ["class"] = "my-custom-class",
            ["data-testid"] = "notes-textarea",
        };

        var cut = Render<SteconTextarea>(p => p.Add(x => x.AdditionalAttributes, attributes));

        var textarea = cut.Find("textarea");
        Assert.Contains("stecon-textarea", textarea.ClassList);
        Assert.Contains("my-custom-class", textarea.ClassList);
        Assert.Equal("notes-textarea", textarea.GetAttribute("data-testid"));
        Assert.Equal(1, textarea.OuterHtml.Split("class=\"").Length - 1);
        Assert.Equal(2, attributes.Count);
    }
}
