# 07 --- Sidebar Specification

## Classification

Sidebar is an **Organism**.

## Component

`SteconSidebar`

## Variants

-   `Workflow` --- primary STECON Workflow-inspired navigation.
-   `Compact` --- icon-forward, space-efficient navigation.
-   `Grouped` --- labeled navigation sections.
-   `Nested` --- hierarchical navigation.
-   `Floating` --- modern inset/floating treatment.
-   `Minimal` --- reduced visual chrome.
-   `MobileDrawer` --- mobile presentation; normally selected
    responsively rather than manually.

## Required states

Expanded, Collapsed, Hover, Active, FocusVisible, Disabled item, Light,
Dark, Mobile open/closed.

## Required capabilities

-   Logo/brand slot.
-   Menu items.
-   Icons.
-   Active route.
-   Optional badges.
-   Optional grouped sections.
-   Optional nested children.
-   Optional user/footer area.
-   Collapsible behavior.
-   Tooltips for icon-only collapsed navigation.
-   Keyboard navigation.
-   Responsive mobile Drawer behavior.

## Suggested API

``` razor
<SteconSidebar
    Variant="SidebarVariant.Workflow"
    Items="@MenuItems"
    Collapsible="true"
    DefaultCollapsed="false"
    BrandLogo="/brand/stecon-logo.png" />
```

## Data model direction

Use a typed navigation model with Id, Label, Icon, Href, Badge,
Disabled, Children, and optional Group. Do not bind the component to an
application's database entity.

## Gallery

`Stecon.UI.Demo` must provide a Sidebar Gallery where developers can
switch variant, expanded/collapsed, light/dark, logo visibility,
user/footer visibility, and viewport preview.

## Acceptance

All variants must use the same token system and menu model. A variant
must not be implemented by copying the entire component.
