using Bunit;

namespace Stecon.UI.Tests;

public class SteconBadgeTests : BunitContext
{
    [Theory]
    [InlineData(BadgeVariant.Neutral, "stecon-badge--neutral")]
    [InlineData(BadgeVariant.Danger, "stecon-badge--danger")]
    public void RendersVariantClass_AndContent(BadgeVariant variant, string expectedClass)
    {
        var cut = Render<SteconBadge>(p => p
            .Add(x => x.Variant, variant)
            .AddChildContent("7"));

        var span = cut.Find("span.stecon-badge");
        Assert.Contains(expectedClass, span.ClassList);
        Assert.Equal("7", span.TextContent.Trim());
    }

    [Fact]
    public void DefaultsToNeutralVariant()
    {
        var cut = Render<SteconBadge>(p => p.AddChildContent("New"));

        Assert.Contains("stecon-badge--neutral", cut.Find("span.stecon-badge").ClassList);
    }
}
