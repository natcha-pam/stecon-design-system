namespace Stecon.UI;

/// <summary>
/// Row-height/padding scale for <see cref="SteconDataTable{TItem}"/>. Deliberately not
/// <see cref="ControlSize"/> - table density applies uniformly across many rows and
/// interacts with line-height differently than a single control's own height/padding scale.
/// </summary>
public enum DataTableDensity
{
    Compact,
    Comfortable,
    Spacious
}
