using System.Globalization;

namespace Stecon.UI;

/// <summary>
/// Pure, side-effect-free helpers that build and evolve <see cref="SteconTableLayoutState"/>
/// against the current column definitions (<see cref="SteconColumnDescriptor"/>). Column
/// definitions describe WHAT a column is; everything here only describes HOW it is currently
/// displayed. None of these methods mutate their inputs - each returns a new state, so callers
/// (e.g. <see cref="SteconDataTableColumnSettings"/>) stay a fully controlled Layout/LayoutChanged
/// consumer, matching the SelectedKeys/SelectedKeysChanged and Sort/SortChanged conventions
/// already used elsewhere in the table.
/// </summary>
public static class SteconTableLayoutReconciler
{
    /// <summary>Fallback pixel width for a pinned column that has no resolvable numeric width - mirrors the existing hardcoded <c>.stecon-data-table__select-col</c> precedent rather than inventing a new Foundation token.</summary>
    public const double DefaultPinnedColumnWidthPx = 160;

    /// <summary>Absolute safety clamp used only when a column does not declare a parseable pixel Min/MaxWidth.</summary>
    public const double MinColumnWidthPx = 60;

    /// <summary>Absolute safety clamp used only when a column does not declare a parseable pixel Min/MaxWidth.</summary>
    public const double MaxColumnWidthPx = 640;

    /// <summary>Increment used by the keyboard-accessible widen/narrow controls.</summary>
    public const double ResizeStepPx = 16;

    /// <summary>
    /// Produces a safe, deterministic layout from a possibly stale/absent saved state and the
    /// current column definitions. Removed columns are dropped silently. Missing columns are
    /// appended in declaration order. <c>CanHide=false</c>/<c>CanPin=false</c> always win over a
    /// saved override. Order values are always renumbered densely with no gaps/duplicates.
    /// </summary>
    public static SteconTableLayoutState Reconcile(SteconTableLayoutState? saved, IReadOnlyList<SteconColumnDescriptor> descriptors)
    {
        var savedByKey = saved?.Columns.ToDictionary(c => c.Key) ?? new Dictionary<string, SteconColumnLayoutState>();

        // Preserve each known column's own stored Order as the tie-break (never its position in the
        // saved list) - list position drifts whenever a sibling group's reflow compacts around it,
        // which would otherwise corrupt later pin-group append order. Unseen columns (new columns)
        // are appended in declaration order at the end - deterministic, never duplicated, never dropped.
        var known = descriptors.Where(d => savedByKey.ContainsKey(d.Key));
        var unseen = descriptors.Where(d => !savedByKey.ContainsKey(d.Key));
        var orderedDescriptors = known
            .OrderBy(d => savedByKey[d.Key].Order)
            .Concat(unseen.OrderBy(d => d.DeclarationOrder))
            .ToList();

        var result = new List<SteconColumnLayoutState>(orderedDescriptors.Count);
        for (var i = 0; i < orderedDescriptors.Count; i++)
        {
            var descriptor = orderedDescriptors[i];
            savedByKey.TryGetValue(descriptor.Key, out var existing);

            var visible = descriptor.CanHide ? existing?.Visible ?? descriptor.DefaultVisible : true;
            var pin = descriptor.CanPin ? existing?.Pin ?? SteconColumnPin.None : SteconColumnPin.None;
            var widthPx = existing?.WidthPx is double savedWidth
                ? ClampWidth(savedWidth, descriptor)
                : descriptor.DefaultWidthPx;

            result.Add(new SteconColumnLayoutState(descriptor.Key, visible, i, pin, widthPx));
        }

        // Expected visual order: left-pinned, then unpinned, then right-pinned - deterministic
        // regardless of stored Order, and re-numbered densely so groups stay contiguous.
        var visualOrder = result
            .OrderBy(c => PinRank(c.Pin))
            .ThenBy(c => c.Order)
            .Select((c, i) => c with { Order = i })
            .ToList();

        return new SteconTableLayoutState(
            visualOrder,
            saved?.Density ?? DataTableDensity.Comfortable,
            saved?.FontSize ?? SteconTableFontSize.Default,
            saved?.WrapMode ?? SteconTableWrapMode.Wrap);
    }

    /// <summary>Columns sorted for rendering: left-pinned, then unpinned, then right-pinned, each group in personalized order.</summary>
    public static IReadOnlyList<SteconColumnLayoutState> VisualOrder(SteconTableLayoutState layout) =>
        layout.Columns.OrderBy(c => PinRank(c.Pin)).ThenBy(c => c.Order).ToList();

    /// <summary>Resolves the effective pixel width for offset/sizing math: personalized width, else a parsed px definition default, else <see cref="DefaultPinnedColumnWidthPx"/> (pinned columns only need a resolvable width; unpinned columns keep using their free-form CSS Width).</summary>
    public static double ResolvePinnedWidthPx(SteconColumnLayoutState column, SteconColumnDescriptor descriptor) =>
        column.WidthPx ?? descriptor.DefaultWidthPx ?? DefaultPinnedColumnWidthPx;

    /// <summary>Parses a literal <c>"NNpx"</c> CSS length. Other units (rem/%/etc.) are not convertible to a definite pixel value without DOM measurement, so they are treated as unresolved - a disclosed, documented scoping limit.</summary>
    public static bool TryParsePixels(string? cssLength, out double pixels)
    {
        pixels = 0;
        if (string.IsNullOrWhiteSpace(cssLength) || !cssLength.EndsWith("px", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return double.TryParse(cssLength[..^2], NumberStyles.Float, CultureInfo.InvariantCulture, out pixels);
    }

    public static SteconTableLayoutState WithColumnVisible(SteconTableLayoutState current, string key, bool visible, IReadOnlyList<SteconColumnDescriptor> descriptors)
    {
        var descriptor = descriptors.FirstOrDefault(d => d.Key == key);
        if (descriptor is null || !descriptor.CanHide)
        {
            return current;
        }

        return Reconcile(ReplaceColumn(current, key, c => c with { Visible = visible }), descriptors);
    }

    public static SteconTableLayoutState WithColumnPin(SteconTableLayoutState current, string key, SteconColumnPin pin, IReadOnlyList<SteconColumnDescriptor> descriptors)
    {
        var descriptor = descriptors.FirstOrDefault(d => d.Key == key);
        if (descriptor is null || !descriptor.CanPin)
        {
            return current;
        }

        // Bump to the end of the global order whenever the pin group actually changes, so it lands
        // after every existing member of its new group - otherwise a sibling's later reflow within
        // the *previous* (e.g. unpinned) group could leave a stale Order value that sorts ahead of
        // columns pinned to the same group earlier, silently reversing the user's pin sequence.
        var maxOrder = current.Columns.Count > 0 ? current.Columns.Max(c => c.Order) : -1;
        var updated = ReplaceColumn(current, key, c => c.Pin == pin ? c : c with { Pin = pin, Order = maxOrder + 1 });
        return Reconcile(updated, descriptors);
    }

    /// <summary>Swaps <paramref name="key"/> with its visual neighbor in the requested direction. A no-op at a pin-group boundary - Move Left/Right never crosses pin groups, keeping pin groups coherent.</summary>
    public static SteconTableLayoutState MoveColumn(SteconTableLayoutState current, string key, int direction, IReadOnlyList<SteconColumnDescriptor> descriptors)
    {
        var visual = VisualOrder(current).ToList();
        var index = visual.FindIndex(c => c.Key == key);
        var neighborIndex = index + direction;
        if (index < 0 || neighborIndex < 0 || neighborIndex >= visual.Count)
        {
            return current;
        }

        if (visual[neighborIndex].Pin != visual[index].Pin)
        {
            return current; // would cross a pin-group boundary
        }

        (visual[index], visual[neighborIndex]) = (visual[neighborIndex] with { Order = visual[index].Order }, visual[index] with { Order = visual[neighborIndex].Order });

        return Reconcile(current with { Columns = visual }, descriptors);
    }

    public static SteconTableLayoutState WithColumnWidth(SteconTableLayoutState current, string key, double widthPx, IReadOnlyList<SteconColumnDescriptor> descriptors)
    {
        var descriptor = descriptors.FirstOrDefault(d => d.Key == key);
        if (descriptor is null)
        {
            return current;
        }

        var clamped = ClampWidth(widthPx, descriptor);
        return Reconcile(ReplaceColumn(current, key, c => c with { WidthPx = clamped }), descriptors);
    }

    public static SteconTableLayoutState ResizeColumn(SteconTableLayoutState current, string key, double deltaPx, IReadOnlyList<SteconColumnDescriptor> descriptors)
    {
        var descriptor = descriptors.FirstOrDefault(d => d.Key == key);
        var existing = current.Columns.FirstOrDefault(c => c.Key == key);
        if (descriptor is null || existing is null)
        {
            return current;
        }

        var baseWidth = ResolvePinnedWidthPx(existing, descriptor);
        return WithColumnWidth(current, key, baseWidth + deltaPx, descriptors);
    }

    /// <summary>Restores a single column to its declared defaults, leaving every other column's personalization untouched.</summary>
    public static SteconTableLayoutState ResetColumn(SteconTableLayoutState current, string key, IReadOnlyList<SteconColumnDescriptor> descriptors)
    {
        var descriptor = descriptors.FirstOrDefault(d => d.Key == key);
        if (descriptor is null)
        {
            return current;
        }

        var reset = new SteconColumnLayoutState(key, descriptor.DefaultVisible, 0, SteconColumnPin.None, descriptor.DefaultWidthPx);
        return Reconcile(ReplaceColumn(current, key, _ => reset), descriptors);
    }

    /// <summary>Returns the declared-default layout for the given columns - also what "Reset all" restores.</summary>
    public static SteconTableLayoutState ResetAll(IReadOnlyList<SteconColumnDescriptor> descriptors) => Reconcile(null, descriptors);

    public static SteconTableLayoutState WithDensity(SteconTableLayoutState current, DataTableDensity density) => current with { Density = density };

    public static SteconTableLayoutState WithFontSize(SteconTableLayoutState current, SteconTableFontSize fontSize) => current with { FontSize = fontSize };

    public static SteconTableLayoutState WithWrapMode(SteconTableLayoutState current, SteconTableWrapMode wrapMode) => current with { WrapMode = wrapMode };

    private static double ClampWidth(double widthPx, SteconColumnDescriptor descriptor)
    {
        var min = descriptor.MinWidthPx ?? MinColumnWidthPx;
        var max = descriptor.MaxWidthPx ?? MaxColumnWidthPx;
        if (max < min)
        {
            max = min;
        }

        return Math.Clamp(widthPx, min, max);
    }

    private static SteconTableLayoutState ReplaceColumn(SteconTableLayoutState current, string key, Func<SteconColumnLayoutState, SteconColumnLayoutState> update)
    {
        var existing = current.Columns.FirstOrDefault(c => c.Key == key);
        if (existing is null)
        {
            return current;
        }

        var columns = current.Columns.Select(c => c.Key == key ? update(c) : c).ToList();
        return current with { Columns = columns };
    }

    private static int PinRank(SteconColumnPin pin) => pin switch
    {
        SteconColumnPin.Left => 0,
        SteconColumnPin.Right => 2,
        _ => 1,
    };
}
