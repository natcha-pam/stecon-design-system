namespace Stecon.UI;

/// <summary>
/// Personalized presentation state for a single column, identified by its explicit, stable
/// <see cref="SteconDataColumn{TItem}.Key"/> - never by title, index, or reflection.
/// Contains no <c>RenderFragment</c>, component reference, or business data, so it stays
/// serialization-friendly (see <see cref="SteconTableLayoutState"/>).
/// </summary>
public sealed record SteconColumnLayoutState(
    string Key,
    bool Visible = true,
    int Order = 0,
    SteconColumnPin Pin = SteconColumnPin.None,
    double? WidthPx = null);
