namespace Stecon.UI;

/// <summary>Why an overlay (Modal/Drawer) transitioned to closed. Shared by both to avoid duplicate enums.</summary>
public enum OverlayCloseReason
{
    Backdrop,
    Escape,
    CloseButton,
    Programmatic
}
