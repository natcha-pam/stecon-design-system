# 09 --- Testing & Acceptance

## Every PR

Run:

``` bash
dotnet build
dotnet test
```

## Component acceptance

Verify: - Public API behavior. - Variant rendering. - Important
callbacks. - Disabled/loading behavior where relevant. - Accessibility
semantics where practical. - No unexpected exceptions with optional
content omitted.

## Visual/manual matrix

At minimum: - Light / Dark - 375 / 768 / 1440 px - Keyboard-only
interaction - Expanded / Collapsed for navigation - Long labels -
Empty/large menu sets where relevant

## First milestone acceptance

Foundation + Sidebar + AppLayout + Demo Gallery is accepted when all
Sidebar variants can be previewed from one gallery, mobile navigation
works, themes work, keyboard focus is visible, build/tests pass, and no
variant duplicates the core navigation implementation.
