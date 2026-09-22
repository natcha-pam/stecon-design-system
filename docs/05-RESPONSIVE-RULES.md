# 05 --- Responsive Rules

## Required verification widths

-   Mobile: 375px
-   Tablet: 768px
-   Desktop: 1440px

These are verification targets, not the only supported widths.

## Behavior

-   Desktop Sidebar may be expanded or collapsed.
-   Tablet may default to compact/collapsed depending on available
    width.
-   Mobile navigation becomes a Drawer.
-   Tables preserve critical columns and may horizontally scroll rather
    than compress into unreadable layouts.
-   Forms stack when horizontal space is insufficient.
-   Modals must remain usable on mobile and may become near-full-screen.

## Rule

Responsive behavior belongs to the component/template specification.
Consuming pages must not patch broken responsiveness with one-off CSS.
