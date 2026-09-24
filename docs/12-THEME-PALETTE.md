# 12 --- Theme & Palette (M3)

## Two independent dimensions

Appearance = Mode x Palette.

-   **Mode** (`ThemeMode`): Light, Dark. Controls luminance/surface
    behavior. Unchanged from M1A.
-   **Palette** (`SteconPalette`): Stecon (default), Romantic, Evening,
    Sunset, Purple, Peach, Peacock. Controls accent
    personality/data-visualization identity.

Changing one never changes the other. There is no 14-value combined
enum.

## Palette provenance

Palette names are direction words. Colors are STECON-safe choices
*inspired by* owner-supplied visual references, not official STECON
brand color specifications. The STECON logo and its provisional brand
tokens (`--stecon-brand-*`) are unaffected by palette selection.

## Token architecture

Each palette defines exactly one set of accent values per Mode:

```css
--stecon-palette-accent
--stecon-palette-accent-hover
--stecon-palette-accent-active
--stecon-palette-accent-soft
--stecon-palette-accent-on
--stecon-palette-accent-on-soft
```

`--stecon-interactive-default/hover/active` and `--stecon-focus-ring`
**alias** `--stecon-palette-accent*` (see `themes/light.css`/`dark.css`)
so palette selection changes primary buttons, selected controls, and
focus rings without a second, drift-prone set of values.

`--stecon-palette-accent-on-soft` exists because a single hue can't
serve two different contrast needs: the solid-fill role needs to stay
dark/saturated enough for white on-interactive text, while text drawn
on the light/dark *-soft* tinted surface needs a different luminance.
This was found empirically via `PaletteContrastTests`, not assumed.

Selectors combine Mode and Palette:

```css
[data-theme='light'][data-palette='romantic'] { ... }
[data-theme='dark'][data-palette='romantic'] { ... }
```

STECON is the fallback (no `[data-palette='stecon']` override exists -
its values live directly in `themes/light.css`/`dark.css`), so removing
or omitting the palette attribute always renders STECON, matching the
default.

## Semantic color isolation

Success/Warning/Danger/Info (`--stecon-status-*`) are a completely
separate token family, never aliased to `--stecon-palette-accent*`.
Danger buttons use `--stecon-status-danger-strong`, never the palette
accent - selecting any palette can never recolor Danger, Warning,
Success, or Info.

## Hidden accent coupling found and fixed

Two pre-existing spots referenced `--stecon-status-info*` for
non-Info-semantic purposes (selected navigation, selected DataTable
row) - a latent coupling that predates M3. Refactored to
`--stecon-palette-accent-soft`/`--stecon-palette-accent-on-soft`
(Sidebar active item, Sidebar minimal-variant indicator, Sidebar
collapse-toggle focus ring) and `--stecon-palette-accent-soft`
(DataTable selected row), so "selected" now follows the palette as
required, while `SteconBadge`/`SteconAlert`/`SteconToast` Info variants
(genuine semantic Info) were left untouched.

## Chart-series tokens

`--stecon-chart-series-1..6` are deliberately **Mode-only, not
palette-specific** - defined once in `themes/light.css`/`dark.css` -
so data-series color coding stays stable regardless of which UI accent
palette is selected. M3 prepares these tokens only; no chart engine is
implemented (later milestone).

## Contrast policy

`PaletteContrastTests` validates every Mode x Palette pair
deterministically (WCAG relative-luminance formula, no third-party
package):

-   Resting, Hover, and Active accent vs white on-interactive text:
    >=4.5:1 each (WCAG AA normal text - no large-text exception).
-   Accent text on its own *-soft* surface: >=4.5:1.

Where a reference-inspired color failed, it was adjusted (see
`SteconAppearanceSettings`/theme CSS comments for exact per-palette
notes) - accessibility won over color fidelity to the visual
references.

## Appearance state and persistence

`SteconAppearanceState(ThemeMode Mode, SteconPalette Palette)` is a
plain record - JSON-serializable, contains no business data and no
`SteconTableLayoutState` reference. M3 does not persist it
(no localStorage/session/database) - that is explicitly future work.

## Appearance Settings

`SteconAppearanceSettings` is fully controlled
(`Appearance`/`AppearanceChanged`), takes zero injected services, and
invents no visible text - every label comes from the required
`SteconAppearanceSettingsLabels`. It composes `SteconRadioGroup`,
`SteconRadio`, and `SteconButton` only. Palette swatches are real
radio inputs with a visible name and a `CheckCircle` icon on the
selected swatch (not color-only).

## Reset

Reset restores Palette to `SteconPalette.Stecon` only - Mode (the
user's own Light/Dark preference) is left untouched. This is
independent of `SteconTableLayoutState`'s own Reset - appearance and
table personalization never interact.
