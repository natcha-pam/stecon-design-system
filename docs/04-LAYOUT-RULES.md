# 04 --- Layout Rules

## App shell

`SteconAppLayout` composes navigation and content. It may host Sidebar,
Topbar, page header, and main content.

## Layout responsibilities

-   Sidebar controls navigation presentation.
-   Topbar controls global application actions.
-   Templates control page composition.
-   Pages supply business content.

## Density

Enterprise screens may be information-dense but must preserve readable
hierarchy and usable hit targets.

## Scrolling

Prefer one predictable primary content scroll region. Data grids may
have controlled horizontal scrolling on narrow screens.

## Width

Avoid page-specific fixed widths unless the template requires them. Use
responsive containers and tokenized gaps.

## Z-index

Define a central z-index scale for sticky headers, sidebar, dropdowns,
drawers, modals, and toasts. Do not invent arbitrary large z-index
values.
