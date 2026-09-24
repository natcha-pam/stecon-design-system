namespace Stecon.UI;

/// <summary>
/// Serializable presentation ("Saved View") state for a <see cref="SteconDataTable{TItem}"/>:
/// column visibility/order/pin/width plus table-level density, font size, and wrap mode.
/// Deliberately excludes business data, sorting, filters, and selection - those are owned by
/// the application, not by presentation layout. Round-trips through <c>System.Text.Json</c>
/// with no custom converters (see SteconDataTableTests serialization tests).
/// </summary>
public sealed record SteconTableLayoutState(
    IReadOnlyList<SteconColumnLayoutState> Columns,
    DataTableDensity Density = DataTableDensity.Comfortable,
    SteconTableFontSize FontSize = SteconTableFontSize.Default,
    SteconTableWrapMode WrapMode = SteconTableWrapMode.Wrap);
