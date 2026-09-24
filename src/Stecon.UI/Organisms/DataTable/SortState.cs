namespace Stecon.UI;

/// <summary>Requested sort for a single column. Null <see cref="SteconDataTable{TItem}.Sort"/> means unsorted.</summary>
public sealed record SortState(string ColumnKey, SortDirection Direction);
