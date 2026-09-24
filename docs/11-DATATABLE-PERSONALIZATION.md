# 11 --- DataTable Personalization (M2B)

## Scope

M2B adds presentation-only personalization on top of the M2 DataTable
core: show/hide, pin left/right, reorder, resize, density, font size,
and wrap mode. It does not add data persistence, business logic, or a
new visual system.

## Column identity

Personalization is keyed exclusively by `SteconDataColumn<TItem>.Key`
(explicit, required, never derived from `Title`, index, `SortField`, or
reflection). This is the same identity M2 already established for
duplicate-key validation.

## State ownership

Column definitions describe **what** a column is (`Key`, `Title`,
`CellTemplate`, `CanHide`, `CanPin`, declared `Width`, declared
`Visible`). They never change based on personalization.

Personalization describes **how** it is currently displayed
(`SteconTableLayoutState` / `SteconColumnLayoutState`). It never
mutates business objects and is never encoded onto column definitions.

## Layout model

```csharp
public sealed record SteconColumnLayoutState(
    string Key, bool Visible = true, int Order = 0,
    SteconColumnPin Pin = SteconColumnPin.None, double? WidthPx = null);

public sealed record SteconTableLayoutState(
    IReadOnlyList<SteconColumnLayoutState> Columns,
    DataTableDensity Density = DataTableDensity.Comfortable,
    SteconTableFontSize FontSize = SteconTableFontSize.Default,
    SteconTableWrapMode WrapMode = SteconTableWrapMode.Wrap);
```

Both are plain records of primitive/enum types only - no
`RenderFragment`, component reference, `ElementReference`, or business
data - so they round-trip through `System.Text.Json` with no custom
converters. This is the "Saved View" representation; **persistence
(database/localStorage) is explicitly deferred to a later milestone.**

## Ownership / binding

`SteconDataTable<TItem>.Layout` is a controlled, read-only input - the
table reconciles it defensively for rendering but never mutates or
pushes a corrected copy back. All personalization edits are made
through `SteconDataTableColumnSettings`'s own `Layout`/`LayoutChanged`,
which the consumer binds to the same state as the table's `Layout`.
This keeps a single source of truth with no hidden global state and no
callback loops. When `Layout` is `null`, the table renders exactly as
M2 did (column declaration order/visibility/width only).

## Reconciliation

`SteconTableLayoutReconciler.Reconcile(saved, descriptors)` is a pure
function that produces a safe, deterministic layout from a possibly
stale saved state and the table's current columns:

- Unknown saved keys (removed columns) are dropped silently.
- Columns present in the definitions but absent from saved state are
  appended in declaration order.
- `CanHide=false`/`CanPin=false` always win over a saved override.
- Widths are clamped to the column's declared Min/MaxWidth (parsed as
  literal `"NNpx"` only - other units are not convertible without DOM
  measurement, a disclosed scoping limit) or an absolute safety range.
- Order is preserved by each column's own stored `Order` value (never
  by transient list position, which can drift when a sibling group
  reflows) and is always re-derived as a dense, duplicate-free
  sequence.

## Pin semantics

`SteconColumnPin`: `None`, `Left`, `Right`. Visual order is always
**left-pinned, then unpinned, then right-pinned**, each group in its
own personalized order - enforced by sorting on `(pinGroupRank, Order)`
every time, so pin groups stay contiguous no matter how columns are
reordered.

Pinning is implemented with CSS `position: sticky` only - no JS scroll
listeners, no `ResizeObserver`. A pinned column without a resolvable
pixel width falls back to a documented constant,
`SteconTableLayoutReconciler.DefaultPinnedColumnWidthPx` (160px),
mirroring the existing hardcoded `.stecon-data-table__select-col`
precedent rather than inventing a new Foundation token. Offsets are
computed by summing the resolved pixel widths of preceding
(left) / following (right) **visible** pinned columns only - a hidden
pinned column contributes no offset gap. The structural selection
column (when present) is always sticky-left at offset 0; data columns
are never assigned a fake business key for this.

## Reordering

Move Left/Right swap a column with its immediate visual neighbor and
are a no-op at a pin-group boundary (reordering never crosses pin
groups). Drag-and-drop is deliberately not implemented in M2B - it
would require new JS/pointer-capture architecture; the accessible
button-driven alternative is the primary mechanism, per the approved
M2B scope.

## Resizing

Resize is keyboard/click-accessible only (Widen/Narrow buttons in
`SteconDataTableColumnSettings`, `SteconTableLayoutReconciler.ResizeStepPx`
increment) - no drag-resize handles, so no new JS was needed. Widths
are stored in pixels and clamped to the column's declared Min/MaxWidth
(if expressed as literal px) or an absolute safety range.

## Density, font size, wrap mode

- Density reuses the existing M2 `DataTableDensity` enum - no
  duplicate concept was introduced.
- `SteconTableFontSize` (`Small`/`Default`/`Large`) controls table
  typography only, via existing Foundation font-size tokens - never
  global application font size.
- `SteconTableWrapMode` (`Wrap`/`NoWrap`) controls cell text wrapping
  only; the table wrapper's own `overflow-x: auto` still applies, so
  `NoWrap` never disables horizontal scrolling.

Each has a simple, uncontrolled `[Parameter]` on the table for
non-personalized usage (matching the existing `Density` pattern) that
is superseded by `Layout`'s value whenever `Layout` is supplied.

## Reset

`SteconTableLayoutReconciler.ResetColumn`/`ResetAll` return to the
column-definition defaults for one column or the whole table. Neither
reloads the browser nor touches sort/filter/selection state.

## Localization

The library invents no English/Thai user-facing text.
`SteconDataTableSettingsLabels` requires every label explicitly
(mirroring the existing `SteconPaginator` Previous/Next precedent) so
`SteconDataTableColumnSettings` stays language-neutral. The Gallery
supplies its own English labels explicitly.

## Accessibility

Native `<table>`/`<th>`/`<td>` semantics are unchanged - still no
`role="grid"`. Show/hide uses a real checkbox, pin uses a real
`<select>`, reorder/resize/reset use real `<button>`s - nothing is
hover-only. Pin state is additionally exposed via the `<select>`'s
value/label text, never color alone.

## Responsive constraints

If more columns are pinned than fit the viewport, the table does not
attempt automatic unpinning - it keeps horizontal scrolling functional
for the remaining unpinned columns. This is a disclosed usability
constraint, not a defect.
