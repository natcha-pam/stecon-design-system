# 00 --- Project Specification

## Product

**STECON Atomic Design System**

## Problem

STECON web applications repeatedly recreate sidebars, tables, filters,
forms, modals, cards, themes, and responsive behavior. This produces
inconsistent UI and forces developers and AI coding tools to receive the
same visual instructions repeatedly.

## Objective

Build a reusable UI system inspired by the approved STECON Workflow
visual direction, with a single set of design tokens, components,
layouts, documentation, tests, and examples.

## Deliverables

-   `Stecon.UI` --- Razor Class Library containing reusable UI.
-   `Stecon.UI.Demo` --- interactive gallery and playground.
-   `Stecon.UI.Tests` --- automated component/unit tests.
-   `docs/` --- authoritative design and engineering specifications.
-   `skills/STECON-UI-SKILL.md` --- compact instructions for AI coding
    agents.

## Non-goals for v1

-   Business-domain workflows.
-   Database access.
-   Authentication implementation.
-   Application-specific business rules.
-   Replacing each application's domain components.
-   Creating separate visual systems per application.

## Design principles

Consistency, reuse, clarity, dense-but-readable enterprise UI,
accessibility, responsive behavior, predictable APIs, and controlled
extensibility.

## Initial v1 scope

Foundation/tokens, Button, IconButton, Input, Checkbox, Switch, Badge,
Avatar, Tooltip, SearchBox, FormField, StatusBadge, Sidebar, Topbar,
AppLayout, FilterBar, DataTable shell, Modal/Drawer shell, and Demo
Gallery.

## First vertical slice

Implement Foundation + Sidebar + AppLayout + Sidebar Gallery first. This
slice must prove tokens, variants, responsive behavior, dark/light
themes, accessibility, and the component API before expanding the
library.
