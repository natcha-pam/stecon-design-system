using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Stecon.UI;

/// <summary>
/// Data-driven navigation entry consumed by <see cref="SteconSidebar"/>. The
/// consuming application decides which items exist and are visible; this
/// model carries no permission/authorization concept (see AGENTS.md).
/// </summary>
public sealed class SteconNavItem
{
    public required string Id { get; init; }

    public required string Label { get; init; }

    public string? Href { get; init; }

    public NavLinkMatch Match { get; init; } = NavLinkMatch.Prefix;

    public IconName? Icon { get; init; }

    /// <summary>Overrides <see cref="Icon"/> when supplied.</summary>
    public RenderFragment? IconContent { get; init; }

    public string? Badge { get; init; }

    public BadgeVariant BadgeVariant { get; init; } = BadgeVariant.Neutral;

    public bool Disabled { get; init; }

    /// <summary>Group label used only by <see cref="SidebarVariant.Grouped"/>.</summary>
    public string? Group { get; init; }

    /// <summary>Nested children rendered only by <see cref="SidebarVariant.Nested"/>.</summary>
    public IReadOnlyList<SteconNavItem>? Children { get; init; }

    /// <summary>
    /// Explicit active-state override. Null uses automatic URI matching via
    /// <see cref="Href"/>/<see cref="Match"/>; true/false forces the state
    /// for workflows that cannot be represented by the URL alone.
    /// </summary>
    public bool? IsActive { get; init; }
}
