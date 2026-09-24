namespace Stecon.UI;

/// <summary>
/// Modal viewport-relative sizing. Deliberately not <see cref="ControlSize"/> -
/// modal sizing models max-width breakpoints (including Xl/Fullscreen), not the
/// inline control height/padding scale ControlSize represents.
/// </summary>
public enum ModalSize
{
    Sm,
    Md,
    Lg,
    Xl,
    Fullscreen
}
