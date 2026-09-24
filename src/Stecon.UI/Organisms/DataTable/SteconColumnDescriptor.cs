namespace Stecon.UI;

/// <summary>
/// Type-erased, non-generic snapshot of a registered <see cref="SteconDataColumn{TItem}"/>'s
/// identity and personalization permissions - what the reconciler and Column Settings UI need,
/// without requiring either to be generic over <c>TItem</c>.
/// </summary>
public sealed record SteconColumnDescriptor(
    string Key,
    string? Title,
    bool CanHide,
    bool CanPin,
    int DeclarationOrder,
    bool DefaultVisible,
    double? DefaultWidthPx,
    double? MinWidthPx = null,
    double? MaxWidthPx = null);
