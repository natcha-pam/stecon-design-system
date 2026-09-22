# 02 --- Design Tokens

## Principle

Components consume semantic tokens. Raw brand colors must not be
scattered through component CSS.

## Token groups

-   Brand
-   Surface
-   Text
-   Border
-   Semantic status
-   Typography
-   Spacing
-   Radius
-   Shadow
-   Breakpoint
-   Motion
-   Z-index

## Brand reference

The supplied STECON logo contains red, blue, and white. Exact corporate
values have not been verified against an official brand guideline.

Use provisional aliases until brand approval:

``` css
:root {
  --stecon-brand-red: #ef1b24;   /* provisional */
  --stecon-brand-blue: #33489b;  /* provisional */
  --stecon-brand-white: #ffffff;

  --stecon-surface-page: #f6f7f9;
  --stecon-surface-card: #ffffff;
  --stecon-text-primary: #172033;
  --stecon-text-secondary: #667085;
  --stecon-border-default: #dfe3e8;

  --stecon-space-1: 0.25rem;
  --stecon-space-2: 0.5rem;
  --stecon-space-3: 0.75rem;
  --stecon-space-4: 1rem;
  --stecon-space-6: 1.5rem;
  --stecon-space-8: 2rem;

  --stecon-radius-sm: 0.375rem;
  --stecon-radius-md: 0.625rem;
  --stecon-radius-lg: 0.875rem;
}
```

The two brand hex values above are intentionally provisional visual
approximations and must be replaced if an official STECON palette is
supplied.

## Typography (Foundation v1)

``` css
:root {
  --stecon-font-family:
    "Noto Sans Thai", "Noto Sans", system-ui, -apple-system,
    BlinkMacSystemFont, "Segoe UI", sans-serif;

  --stecon-font-size-xs: 0.75rem;
  --stecon-font-size-sm: 0.875rem;
  --stecon-font-size-md: 1rem;
  --stecon-font-size-lg: 1.125rem;
  --stecon-font-size-xl: 1.25rem;
  --stecon-font-size-2xl: 1.5rem;
  --stecon-font-size-3xl: 1.875rem;

  --stecon-font-weight-regular: 400;
  --stecon-font-weight-medium: 500;
  --stecon-font-weight-semibold: 600;
  --stecon-font-weight-bold: 700;

  --stecon-line-height-tight: 1.25;
  --stecon-line-height-normal: 1.5;
  --stecon-line-height-relaxed: 1.75;
}
```

## Shadow (Foundation v1)

``` css
:root {
  --stecon-shadow-sm: 0 1px 2px rgb(0 0 0 / 0.05);
  --stecon-shadow-md: 0 4px 12px rgb(0 0 0 / 0.08);
  --stecon-shadow-lg: 0 12px 28px rgb(0 0 0 / 0.12);
}
```

## Motion (Foundation v1)

``` css
:root {
  --stecon-duration-fast: 120ms;
  --stecon-duration-normal: 200ms;
  --stecon-duration-slow: 300ms;

  --stecon-ease-standard: cubic-bezier(0.2, 0, 0, 1);
  --stecon-ease-enter: cubic-bezier(0, 0, 0.2, 1);
  --stecon-ease-exit: cubic-bezier(0.4, 0, 1, 1);
}
```

Durations collapse to `0ms` under `prefers-reduced-motion: reduce` (see
docs/06-ACCESSIBILITY.md).

## Z-index (Foundation v1)

Categories mirror docs/04-LAYOUT-RULES.md. Components must consume
these tokens and must not declare arbitrary stacking values.

``` css
:root {
  --stecon-z-base: 0;
  --stecon-z-sticky: 100;
  --stecon-z-sidebar: 200;
  --stecon-z-dropdown: 300;
  --stecon-z-drawer: 400;
  --stecon-z-modal: 500;
  --stecon-z-toast: 600;
}
```

## Semantic rule

Do not automatically use brand red as `danger` or brand blue as every
interactive state. Define semantic aliases independently so meaning
remains clear and accessible.

## Theme rule

Light and dark themes redefine semantic tokens. Components must not
contain separate hard-coded light/dark palettes.
