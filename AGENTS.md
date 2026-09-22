# STECON UI --- AI Development Contract

## Mandatory workflow

SPEC → IMPLEMENT → BUILD → TEST → REVIEW

## Source of truth

Use this priority: 1. `docs/*` 2. `skills/STECON-UI-SKILL.md` 3.
Existing `Stecon.UI` components 4. Current task prompt

Do not invent requirements that conflict with specifications.

## UI rules

Before creating UI: 1. Search existing `Stecon.UI` components. 2. Reuse
an existing component whenever possible. 3. Reuse design tokens; do not
hard-code brand values in components. 4. Do not duplicate components
under a different name. 5. Do not create a page-specific design system.
6. Reusable UI belongs in `Stecon.UI`. 7. Application projects compose
components; they do not redefine them. 8. Keep component APIs explicit,
typed, and documented. 9. Preserve accessibility and responsive
behavior. 10. When a requirement is absent or ambiguous, stop and
request a specification decision rather than inventing business/UI
rules.

## Atomic dependency direction

Foundation → Atoms → Molecules → Organisms → Templates → Pages.

A layer may depend only on the same layer when justified or on lower
layers. Lower layers must never depend on higher layers.

## Definition of Done

A reusable component is complete only when: - `dotnet build` passes. -
Automated tests pass. - Demo/example exists. - Light and dark themes are
verified. - 375px, 768px, and 1440px layouts are verified. - Keyboard
navigation and visible focus are verified. - Disabled/loading/error
states are considered where applicable. - No duplicate component or
token was introduced. - Public parameters and variants are documented.
