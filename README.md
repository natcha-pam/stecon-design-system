# STECON Atomic Design System

Reusable UI foundation for STECON internal web applications.

## Goal

Create one consistent design system that can be reused across STECON
applications instead of redesigning common UI for every project.

## Architecture

Foundation → Atoms → Molecules → Organisms → Templates → Pages

## Solution target

-   .NET 10
-   Blazor / Razor Class Library
-   `Stecon.UI` reusable component library
-   `Stecon.UI.Demo` component gallery/playground
-   `Stecon.UI.Tests` automated tests

## AI development workflow

**GPT → Spec / Atomic Rules → GitHub → Copilot → Code/Test → GPT
Review**

Specifications are authoritative. Copilot implements the specification;
it must not invent a competing design system.

## Brand asset

The supplied STECON logo is stored under `src/Stecon.UI/wwwroot/brand/`.

Brand colors in this v1 pack are provisional tokens derived from the
supplied visual reference and must be confirmed against an official
corporate brand guideline before being treated as authoritative.
