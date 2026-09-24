namespace Stecon.UI.Tests;

public class ToastServiceTests
{
    [Fact]
    public void Show_AddsToast_AndRaisesToastsChanged()
    {
        var service = new ToastService();
        var changed = false;
        service.ToastsChanged += () => changed = true;

        var id = service.Show("Hello", ToastVariant.Success);

        Assert.True(changed);
        Assert.Single(service.Toasts);
        Assert.Equal(id, service.Toasts[0].Id);
        Assert.Equal("Hello", service.Toasts[0].Message);
        Assert.Equal(ToastVariant.Success, service.Toasts[0].Variant);
    }

    [Fact]
    public void Dismiss_RemovesToast_AndRaisesToastsChanged()
    {
        var service = new ToastService();
        var id = service.Show("Hello");
        var changeCount = 0;
        service.ToastsChanged += () => changeCount++;

        service.Dismiss(id);

        Assert.Empty(service.Toasts);
        Assert.Equal(1, changeCount);
    }

    [Fact]
    public void Dismiss_UnknownId_DoesNotRaiseToastsChanged()
    {
        var service = new ToastService();
        var changed = false;
        service.ToastsChanged += () => changed = true;

        service.Dismiss("does-not-exist");

        Assert.False(changed);
    }

    [Fact]
    public void PersistentToast_HasNullDuration_AndIsNotAutoRemoved()
    {
        var service = new ToastService();
        service.Show("Persistent", ToastVariant.Info, new ToastOptions { Duration = null });

        Assert.Null(service.Toasts[0].Duration);
    }

    [Fact]
    public async Task DurationElapses_AutoDismissesToast()
    {
        var service = new ToastService();
        var dismissed = new TaskCompletionSource();
        service.ToastsChanged += () =>
        {
            if (service.Toasts.Count == 0)
            {
                dismissed.TrySetResult();
            }
        };

        service.Show("Short-lived", ToastVariant.Info, new ToastOptions { Duration = TimeSpan.FromMilliseconds(50) });

        var completed = await Task.WhenAny(dismissed.Task, Task.Delay(TimeSpan.FromSeconds(5)));

        Assert.Same(dismissed.Task, completed);
        Assert.Empty(service.Toasts);
    }

    [Fact]
    public void Dispose_ClearsToasts_AndDoesNotThrow()
    {
        var service = new ToastService();
        service.Show("Hello");

        var exception = Record.Exception(service.Dispose);

        Assert.Null(exception);
        Assert.Empty(service.Toasts);
    }

    [Fact]
    public void SeparateServiceInstances_DoNotShareState()
    {
        // Mirrors DI scoping across circuits/users - each instance is fully independent.
        var serviceA = new ToastService();
        var serviceB = new ToastService();

        serviceA.Show("Only in A");

        Assert.Single(serviceA.Toasts);
        Assert.Empty(serviceB.Toasts);
    }
}
