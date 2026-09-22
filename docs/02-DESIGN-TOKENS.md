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

## Semantic rule

Do not automatically use brand red as `danger` or brand blue as every
interactive state. Define semantic aliases independently so meaning
remains clear and accessible.

## Theme rule

Light and dark themes redefine semantic tokens. Components must not
contain separate hard-coded light/dark palettes.
