using Bunit;
using Bunit.JSInterop;
using Microsoft.Extensions.DependencyInjection;

namespace Stecon.UI.Tests;

public class SteconToastHostTests : BunitContext
{
    private ToastService SetupHost()
    {
        var module = JSInterop.SetupModule("./_content/Stecon.UI/js/stecon-overlay.js");
        module.SetupVoid("showPopoverHost", _ => true).SetVoidResult();
        module.SetupVoid("hidePopoverHost", _ => true).SetVoidResult();

        var service = new ToastService();
        Services.AddSingleton<IToastService>(service);
        return service;
    }

    [Fact]
    public void RendersOneToast_PerServiceEntry()
    {
        var service = SetupHost();
        service.Show("Hello");

        var cut = Render<SteconToastHost>();

        Assert.Single(cut.FindAll(".stecon-toast"));
    }

    [Fact]
    public void DismissButton_RemovesFromService()
    {
        var service = SetupHost();
        var id = service.Show("Hello");

        var cut = Render<SteconToastHost>();
        cut.Find("button").Click();

        Assert.DoesNotContain(service.Toasts, t => t.Id == id);
    }

    [Fact]
    public void ExceedingMaxVisible_RemovesOldestFirst()
    {
        var service = SetupHost();
        var ids = new List<string>();
        for (var i = 0; i < 8; i++)
        {
            ids.Add(service.Show($"Toast {i}"));
        }

        Render<SteconToastHost>(p => p.Add(x => x.MaxVisible, 5));

        Assert.Equal(5, service.Toasts.Count);
        // the 3 oldest (first added) must be gone, the 5 newest remain
        Assert.DoesNotContain(service.Toasts, t => t.Id == ids[0]);
        Assert.DoesNotContain(service.Toasts, t => t.Id == ids[1]);
        Assert.DoesNotContain(service.Toasts, t => t.Id == ids[2]);
        Assert.Contains(service.Toasts, t => t.Id == ids[7]);
    }

    [Fact]
    public void DefaultMaxVisible_IsFive()
    {
        var service = SetupHost();
        for (var i = 0; i < 6; i++)
        {
            service.Show($"Toast {i}");
        }

        Render<SteconToastHost>();

        Assert.Equal(5, service.Toasts.Count);
    }

    [Fact]
    public async Task DisposeAsync_UnsubscribesFromService()
    {
        var service = SetupHost();
        var cut = Render<SteconToastHost>();

        await cut.Instance.DisposeAsync();

        // After disposal, adding a toast must not throw or attempt a stale render.
        var exception = Record.Exception(() => service.Show("After dispose"));
        Assert.Null(exception);
    }

    [Theory]
    [InlineData(ToastPosition.TopRight, "stecon-toast-host--top-right")]
    [InlineData(ToastPosition.BottomLeft, "stecon-toast-host--bottom-left")]
    public void Position_RendersCorrectClass(ToastPosition position, string expectedClass)
    {
        SetupHost();
        var cut = Render<SteconToastHost>(p => p.Add(x => x.Position, position));

        Assert.Contains(expectedClass, cut.Find(".stecon-toast-host").ClassList);
    }
}
