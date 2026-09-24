using Bunit;
using Bunit.JSInterop;

namespace Stecon.UI.Tests;

public class SteconDrawerTests : BunitContext
{
    private BunitJSModuleInterop SetupOverlayModule()
    {
        var module = JSInterop.SetupModule("./_content/Stecon.UI/js/stecon-overlay.js");
        module.Setup<bool>("showOverlay", _ => true).SetResult(true);
        module.SetupVoid("closeOverlay", _ => true).SetVoidResult();
        module.SetupVoid("disposeOverlay", _ => true).SetVoidResult();
        return module;
    }

    [Fact]
    public void OpeningDrawer_InvokesShowOverlay_NonExclusive()
    {
        var module = SetupOverlayModule();
        Render<SteconDrawer>(p => p
            .Add(x => x.IsOpen, true)
            .AddChildContent("Body"));

        module.VerifyInvoke("showOverlay");
    }

    [Theory]
    [InlineData(DrawerPlacement.Left, "stecon-drawer--left")]
    [InlineData(DrawerPlacement.Right, "stecon-drawer--right")]
    public void Placement_RendersCorrectClass(DrawerPlacement placement, string expectedClass)
    {
        SetupOverlayModule();
        var cut = Render<SteconDrawer>(p => p
            .Add(x => x.Placement, placement)
            .AddChildContent("Body"));

        Assert.Contains(expectedClass, cut.Find("dialog").ClassList);
    }

    [Fact]
    public void CloseButton_InvokesIsOpenChanged_AndOnClose()
    {
        SetupOverlayModule();
        var closedValues = new List<bool>();
        OverlayCloseReason? reason = null;

        var cut = Render<SteconDrawer>(p => p
            .Add(x => x.IsOpen, true)
            .Add(x => x.IsOpenChanged, v => closedValues.Add(v))
            .Add(x => x.OnClose, r => reason = r)
            .AddChildContent("Body"));

        cut.Find("button").Click();

        Assert.Contains(false, closedValues);
        Assert.Equal(OverlayCloseReason.CloseButton, reason);
    }

    [Fact]
    public void AccessibleName_UsesTitleWhenPresent()
    {
        SetupOverlayModule();
        var cut = Render<SteconDrawer>(p => p
            .Add(x => x.Title, "Filters")
            .AddChildContent("Body"));

        var dialog = cut.Find("dialog");
        var titleId = dialog.GetAttribute("aria-labelledby");
        Assert.NotNull(titleId);
        Assert.Equal("Filters", cut.Find("#" + titleId).TextContent);
    }

    [Fact]
    public async Task DisposeAsync_DoesNotThrow()
    {
        SetupOverlayModule();
        var cut = Render<SteconDrawer>(p => p
            .Add(x => x.IsOpen, true)
            .AddChildContent("Body"));

        await cut.Instance.DisposeAsync();
    }
}
