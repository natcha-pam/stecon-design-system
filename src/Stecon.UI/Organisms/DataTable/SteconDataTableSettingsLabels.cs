namespace Stecon.UI;

/// <summary>
/// All user-facing text for <see cref="SteconDataTableColumnSettings"/>. No English/Thai text is
/// invented inside the library - every label is required so the reusable component stays
/// language-neutral, mirroring the existing <c>SteconPaginator</c> Previous/Next precedent.
/// </summary>
public sealed record SteconDataTableSettingsLabels(
    string ColumnsSectionLabel,
    string TableSectionLabel,
    string VisibleLabel,
    string PinNoneLabel,
    string PinLeftLabel,
    string PinRightLabel,
    string MoveLeftLabel,
    string MoveRightLabel,
    string WidenLabel,
    string NarrowLabel,
    string ResetColumnLabel,
    string DensityCompactLabel,
    string DensityComfortableLabel,
    string DensitySpaciousLabel,
    string FontSizeSmallLabel,
    string FontSizeDefaultLabel,
    string FontSizeLargeLabel,
    string WrapLabel,
    string NoWrapLabel,
    string ResetAllLabel);
