using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Stecon.UI.Internal;

/// <summary>
/// Thin wrapper around wwwroot/js/stecon-overlay.js - the project's only JS module,
/// used exclusively for native &lt;dialog&gt;/popover lifecycle glue that Blazor cannot
/// express declaratively (showModal/close/backdrop-click/Escape/scroll-lock/top-layer).
/// Shared by SteconModal, SteconDrawer, and SteconToastHost. Internal - not public API.
/// </summary>
internal sealed class SteconOverlayInterop : IAsyncDisposable
{
    private const string ModulePath = "./_content/Stecon.UI/js/stecon-overlay.js";

    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

    public SteconOverlayInterop(IJSRuntime jsRuntime)
    {
        _moduleTask = new Lazy<Task<IJSObjectReference>>(
            () => jsRuntime.InvokeAsync<IJSObjectReference>("import", ModulePath).AsTask());
    }

    /// <summary>Calls dialog.showModal(). Returns false when exclusiveModal is true and another exclusive dialog is already open (deterministic refusal, never a stacked second modal).</summary>
    public async Task<bool> ShowOverlayAsync(ElementReference dialog, object dotNetRef, bool exclusiveModal, bool closeOnEscape, bool closeOnBackdrop)
    {
        var module = await _moduleTask.Value;
        return await module.InvokeAsync<bool>("showOverlay", dialog, dotNetRef, exclusiveModal, closeOnEscape, closeOnBackdrop);
    }

    public async Task CloseOverlayAsync(ElementReference dialog)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("closeOverlay", dialog);
    }

    public async Task DisposeOverlayAsync(ElementReference dialog)
    {
        if (!_moduleTask.IsValueCreated)
        {
            return;
        }

        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("disposeOverlay", dialog);
    }

    public async Task ShowPopoverHostAsync(ElementReference host)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("showPopoverHost", host);
    }

    public async Task HidePopoverHostAsync(ElementReference host)
    {
        if (!_moduleTask.IsValueCreated)
        {
            return;
        }

        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("hidePopoverHost", host);
    }

    public async ValueTask DisposeAsync()
    {
        if (!_moduleTask.IsValueCreated)
        {
            return;
        }

        var module = await _moduleTask.Value;
        await module.DisposeAsync();
    }
}
