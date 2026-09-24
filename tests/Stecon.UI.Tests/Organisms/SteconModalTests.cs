using Bunit;
using Bunit.JSInterop;
using Microsoft.AspNetCore.Components;

namespace Stecon.UI.Tests;

public class SteconModalTests : BunitContext
{
    private BunitJSModuleInterop SetupOverlayModule(bool showOverlayResult = true)
    {
        var module = JSInterop.SetupModule("./_content/Stecon.UI/js/stecon-overlay.js");
        module.Setup<bool>("showOverlay", _ => true).SetResult(showOverlayResult);
        module.SetupVoid("closeOverlay", _ => true).SetVoidResult();
        module.SetupVoid("disposeOverlay", _ => true).SetVoidResult();
        return module;
    }

    [Fact]
    public void ClosedModal_RendersDialogWithoutOpenAttribute()
    {
        SetupOverlayModule();
        var cut = Render<SteconModal>(p => p.AddChildContent("Body"));

        var dialog = cut.Find("dialog");
        Assert.False(dialog.HasAttribute("open"));
    }

    [Fact]
    public void OpeningModal_InvokesShowOverlay()
    {
        var module = SetupOverlayModule();
        var cut = Render<SteconModal>(p => p
            .Add(x => x.IsOpen, true)
            .AddChildContent("Body"));

        module.VerifyInvoke("showOverlay");
    }

    [Fact]
    public void CloseButton_InvokesIsOpenChanged_AndOnClose()
    {
        SetupOverlayModule();
        var closedValues = new List<bool>();
        OverlayCloseReason? reason = null;

        var cut = Render<SteconModal>(p => p
            .Add(x => x.IsOpen, true)
            .Add(x => x.IsOpenChanged, v => closedValues.Add(v))
            .Add(x => x.OnClose, r => reason = r)
            .AddChildContent("Body"));

        cut.Find("button").Click();

        Assert.Contains(false, closedValues);
        Assert.Equal(OverlayCloseReason.CloseButton, reason);
    }

    [Fact]
    public void ShowCloseButtonFalse_OmitsCloseButton()
    {
        SetupOverlayModule();
        var cut = Render<SteconModal>(p => p
            .Add(x => x.ShowCloseButton, false)
            .AddChildContent("Body"));

        Assert.Empty(cut.FindAll("button"));
    }

    [Fact]
    public void TitleSet_RendersAriaLabelledBy_PointingAtTitleId()
    {
        SetupOverlayModule();
        var cut = Render<SteconModal>(p => p
            .Add(x => x.Title, "My title")
            .AddChildContent("Body"));

        var dialog = cut.Find("dialog");
        var titleId = dialog.GetAttribute("aria-labelledby");
        Assert.NotNull(titleId);
        Assert.Equal(titleId, cut.Find("#" + titleId).Id);
    }

    [Fact]
    public void UseAlertDialogRole_SetsAlertdialogRole()
    {
        SetupOverlayModule();
        var cut = Render<SteconModal>(p => p
            .Add(x => x.UseAlertDialogRole, true)
            .AddChildContent("Body"));

        Assert.Equal("alertdialog", cut.Find("dialog").GetAttribute("role"));
    }

    [Fact]
    public void DefaultRole_IsDialog()
    {
        SetupOverlayModule();
        var cut = Render<SteconModal>(p => p.AddChildContent("Body"));

        Assert.Equal("dialog", cut.Find("dialog").GetAttribute("role"));
    }

    [Fact]
    public void RejectedShowOverlay_RevertsIsOpenChangedToFalse()
    {
        // Simulates the JS-side deterministic refusal (another exclusive modal already active).
        var module = JSInterop.SetupModule("./_content/Stecon.UI/js/stecon-overlay.js");
        module.Setup<bool>("showOverlay", _ => true).SetResult(false);
        module.SetupVoid("closeOverlay", _ => true).SetVoidResult();
        module.SetupVoid("disposeOverlay", _ => true).SetVoidResult();

        var openValues = new List<bool>();
        Render<SteconModal>(p => p
            .Add(x => x.IsOpen, true)
            .Add(x => x.IsOpenChanged, v => openValues.Add(v))
            .AddChildContent("Second"));

        Assert.Contains(false, openValues);
    }

    [Fact]
    public void ConsumerSuppliedClass_IsMergedWithInternalClasses()
    {
        SetupOverlayModule();
        var attributes = new Dictionary<string, object>
        {
            ["class"] = "my-custom-class",
            ["data-testid"] = "profile-modal",
        };

        var cut = Render<SteconModal>(p => p
            .Add(x => x.AdditionalAttributes, attributes)
            .AddChildContent("Body"));

        var dialog = cut.Find("dialog");
        Assert.Contains("stecon-modal", dialog.ClassList);
        Assert.Contains("my-custom-class", dialog.ClassList);
        Assert.Equal("profile-modal", dialog.GetAttribute("data-testid"));
    }

    [Fact]
    public async Task DisposeAsync_DoesNotThrow()
    {
        SetupOverlayModule();
        var cut = Render<SteconModal>(p => p
            .Add(x => x.IsOpen, true)
            .AddChildContent("Body"));

        await cut.Instance.DisposeAsync();
    }
}
