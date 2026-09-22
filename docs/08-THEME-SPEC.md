# 08 --- Theme Specification

## Themes

v1 supports Light and Dark.

## Architecture

Themes override semantic CSS custom properties at a root/theme scope.
Components consume semantic tokens only.

## Requirements

-   Theme switch does not require component-specific JavaScript.
-   User preference can be controlled by consuming applications.
-   Components render correctly in both themes.
-   Brand identity remains recognizable without forcing brand colors
    onto every surface.
-   Status colors retain meaning and sufficient contrast.

## Future extension

Architecture may later support application accent themes, but v1 must
not create separate design systems per product.
