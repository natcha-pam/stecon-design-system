using Bunit;

namespace Stecon.UI.Tests;

public class SteconRadioTests : BunitContext
{
    [Fact]
    public void ConsumerSuppliedClass_IsMergedWithInternalClasses()
    {
        var attributes = new Dictionary<string, object>
        {
            ["class"] = "my-custom-class",
            ["data-testid"] = "standard-radio",
        };

        var cut = Render<SteconRadio<string>>(p => p
            .Add(x => x.Value, "a")
            .Add(x => x.AdditionalAttributes, attributes));

        var input = cut.Find("input");
        Assert.Contains("stecon-radio__input", input.ClassList);
        Assert.Contains("my-custom-class", input.ClassList);
        Assert.Equal("standard-radio", input.GetAttribute("data-testid"));
        Assert.Equal(1, input.OuterHtml.Split("class=\"").Length - 1);
        Assert.Equal(2, attributes.Count);
    }
}
