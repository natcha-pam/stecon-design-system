namespace Stecon.UI;

/// <summary>
/// Explicit, consumer-controlled live-region behavior - never inferred from
/// <see cref="AlertVariant"/>. Default Off is correct for static page content
/// (docs/06-ACCESSIBILITY.md: do not use role=alert indiscriminately).
/// </summary>
public enum AlertLive
{
    Off,
    Polite,
    Assertive
}
