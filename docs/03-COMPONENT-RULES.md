# 03 --- Component Rules

## Naming

Public components use the `Stecon` prefix, for example `SteconButton`,
`SteconSidebar`, `SteconDataTable`.

## API design

-   Prefer enums over magic strings for finite variants.
-   Prefer `RenderFragment` for composable content.
-   Use `EventCallback<T>` for user actions.
-   Keep required parameters explicit.
-   Avoid application services inside visual components.
-   CSS classes are implementation details unless explicitly documented
    as extension points.

## Variants

A visual family is normally one component with typed variants, not
duplicated components.

Example:

``` razor
<SteconSidebar Variant="SidebarVariant.Grouped" Collapsible="true" />
```

Do not create `Sidebar1`, `Sidebar2`, `NewSidebar`, or page-specific
copies.

## Styling

-   Use CSS isolation where practical.
-   Use tokens for colors, spacing, radius, typography, shadow, and
    motion.
-   No inline hard-coded brand colors.
-   Consumer overrides must be intentional and documented.

## States

Where applicable, components define default, hover, active,
focus-visible, disabled, loading, selected, error, and empty states.
