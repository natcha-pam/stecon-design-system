namespace Stecon.UI;

/// <summary>
/// Scoped per circuit/session (mirrors <see cref="IThemeService"/>) - never a
/// static/global event bus, so toasts never leak across users/circuits.
/// </summary>
public interface IToastService
{
    IReadOnlyList<ToastMessage> Toasts { get; }

    event Action? ToastsChanged;

    /// <summary>Returns the new toast's id (usable with <see cref="Dismiss"/>).</summary>
    string Show(string message, ToastVariant variant = ToastVariant.Info, ToastOptions? options = null);

    void Dismiss(string id);
}
