# STECON UI Skill

Use this skill whenever implementing or reviewing UI that consumes or
modifies `Stecon.UI`.

## Before coding

1.  Read `AGENTS.md`.
2.  Read the relevant `docs/*` specifications.
3.  Search existing components and tokens.
4.  Identify the Atomic layer.
5.  State which existing components will be reused.

## Implementation rules

-   Reuse before create.
-   Compose before copy.
-   Tokens before hard-coded values.
-   Typed variants before duplicated components.
-   UI components contain no application business/data-access logic.
-   Do not invent pages, statuses, roles, fields, workflows, colors, or
    variants absent from the specification.
-   If a missing decision materially affects the public component API,
    stop and request a spec decision.

## Sidebar

Use `SteconSidebar` and `SidebarVariant`. Never create a separate
sidebar implementation for each application or variant.

## Quality gate

Before completion:

``` bash
dotnet build
dotnet test
```

Then verify relevant components at 375px, 768px, and 1440px, Light/Dark,
keyboard navigation, focus-visible, and important states.

## Copilot task response

For implementation tasks, report: 1. Spec files read. 2. Files changed.
3. Components/tokens reused. 4. Tests added/updated. 5. Build result. 6.
Test result. 7. Remaining spec questions or known limitations.

Do not claim completion when build or tests fail.
