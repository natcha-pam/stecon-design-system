using Bunit;

namespace Stecon.UI.Tests;

public class SteconAlertTests : BunitContext
{
    [Theory]
    [InlineData(AlertVariant.Info, "stecon-alert--info")]
    [InlineData(AlertVariant.Success, "stecon-alert--success")]
    [InlineData(AlertVariant.Warning, "stecon-alert--warning")]
    [InlineData(AlertVariant.Danger, "stecon-alert--danger")]
    public void RendersVariantClass(AlertVariant variant, string expectedClass)
    {
        var cut = Render<SteconAlert>(p => p
            .Add(x => x.Variant, variant)
            .AddChildContent("Message"));

        Assert.Contains(expectedClass, cut.Find(".stecon-alert").ClassList);
    }

    [Fact]
    public void Dismissible_RendersCloseButton_AndInvokesOnDismiss()
    {
        var dismissed = false;
        var cut = Render<SteconAlert>(p => p
            .Add(x => x.Dismissible, true)
            .Add(x => x.OnDismiss, () => dismissed = true)
            .AddChildContent("Message"));

        cut.Find("button").Click();

        Assert.True(dismissed);
    }

    [Fact]
    public void NotDismissible_OmitsCloseButton()
    {
        var cut = Render<SteconAlert>(p => p.AddChildContent("Message"));

        Assert.Empty(cut.FindAll("button"));
    }

    [Theory]
    [InlineData(AlertLive.Off, null, null)]
    [InlineData(AlertLive.Polite, "status", "polite")]
    [InlineData(AlertLive.Assertive, "alert", "assertive")]
    public void Live_MapsToCorrectRoleAndAriaLive(AlertLive live, string? expectedRole, string? expectedAriaLive)
    {
        var cut = Render<SteconAlert>(p => p
            .Add(x => x.Live, live)
            .AddChildContent("Message"));

        var alert = cut.Find(".stecon-alert");
        Assert.Equal(expectedRole, alert.GetAttribute("role"));
        Assert.Equal(expectedAriaLive, alert.GetAttribute("aria-live"));
    }

    [Fact]
    public void ConsumerSuppliedClass_IsMergedWithInternalClasses()
    {
        var attributes = new Dictionary<string, object>
        {
            ["class"] = "my-custom-class",
            ["data-testid"] = "banner",
        };

        var cut = Render<SteconAlert>(p => p
            .Add(x => x.AdditionalAttributes, attributes)
            .AddChildContent("Message"));

        var alert = cut.Find(".stecon-alert");
        Assert.Contains("stecon-alert", alert.ClassList);
        Assert.Contains("my-custom-class", alert.ClassList);
        Assert.Equal("banner", alert.GetAttribute("data-testid"));
    }
}
