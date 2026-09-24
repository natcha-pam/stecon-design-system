using Bunit;
using Bunit.JSInterop;

namespace Stecon.UI.Tests;

public class SteconConfirmTests : BunitContext
{
    private void SetupOverlayModule()
    {
        var module = JSInterop.SetupModule("./_content/Stecon.UI/js/stecon-overlay.js");
        module.Setup<bool>("showOverlay", _ => true).SetResult(true);
        module.SetupVoid("closeOverlay", _ => true).SetVoidResult();
        module.SetupVoid("disposeOverlay", _ => true).SetVoidResult();
    }

    [Fact]
    public void ConfirmButtonClick_InvokesOnConfirm()
    {
        SetupOverlayModule();
        var confirmed = false;
        var cut = Render<SteconConfirm>(p => p
            .Add(x => x.IsOpen, true)
            .Add(x => x.ConfirmLabel, "Delete")
            .Add(x => x.CancelLabel, "Cancel")
            .Add(x => x.OnConfirm, () => confirmed = true)
            .AddChildContent("Are you sure?"));

        cut.Find("button:last-of-type").Click();

        Assert.True(confirmed);
    }

    [Fact]
    public void CancelButtonClick_InvokesOnCancel_AndCloses()
    {
        SetupOverlayModule();
        var cancelled = false;
        var openValues = new List<bool>();
        var cut = Render<SteconConfirm>(p => p
            .Add(x => x.IsOpen, true)
            .Add(x => x.IsOpenChanged, v => openValues.Add(v))
            .Add(x => x.ConfirmLabel, "Delete")
            .Add(x => x.CancelLabel, "Cancel")
            .Add(x => x.OnCancel, () => cancelled = true)
            .AddChildContent("Are you sure?"));

        cut.Find("button:first-of-type").Click();

        Assert.True(cancelled);
        Assert.Contains(false, openValues);
    }

    [Fact]
    public void DangerVariant_UsesAlertdialogRole_AndDangerButton()
    {
        SetupOverlayModule();
        var cut = Render<SteconConfirm>(p => p
            .Add(x => x.IsOpen, true)
            .Add(x => x.ConfirmLabel, "Delete")
            .Add(x => x.CancelLabel, "Cancel")
            .Add(x => x.Variant, ConfirmVariant.Danger)
            .AddChildContent("Are you sure?"));

        Assert.Equal("alertdialog", cut.Find("dialog").GetAttribute("role"));
        Assert.Contains("stecon-button--danger", cut.Find("button:last-of-type").ClassList);
    }

    [Fact]
    public void Busy_PreventsDuplicateConfirm()
    {
        SetupOverlayModule();
        var confirmCount = 0;
        var cut = Render<SteconConfirm>(p => p
            .Add(x => x.IsOpen, true)
            .Add(x => x.ConfirmLabel, "Delete")
            .Add(x => x.CancelLabel, "Cancel")
            .Add(x => x.Busy, true)
            .Add(x => x.OnConfirm, () => confirmCount++)
            .AddChildContent("Are you sure?"));

        var confirmButton = cut.Find("button:last-of-type");
        Assert.True(confirmButton.HasAttribute("disabled"));
        confirmButton.Click();

        Assert.Equal(0, confirmCount);
    }
}
