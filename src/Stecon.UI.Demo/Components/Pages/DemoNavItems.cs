using Microsoft.AspNetCore.Components.Routing;
using Stecon.UI;

namespace Stecon.UI.Demo.Components.Pages;

/// <summary>Sample navigation datasets for the Sidebar Gallery - demo-only, not part of Stecon.UI.</summary>
internal static class DemoNavItems
{
    public static IReadOnlyList<SteconNavItem> Flat { get; } = new List<SteconNavItem>
    {
        new() { Id = "flat-home", Label = "Home", Href = "sidebar-gallery", Match = NavLinkMatch.All, Icon = IconName.Home },
        new() { Id = "flat-counter", Label = "Counter", Href = "counter", Badge = "3", BadgeVariant = BadgeVariant.Info },
        new() { Id = "flat-weather", Label = "Weather", Href = "weather" },
        new() { Id = "flat-approvals", Label = "Approvals awaiting review", Href = "foundation", Badge = "12", BadgeVariant = BadgeVariant.Warning },
        new() { Id = "flat-archived", Label = "Archived area", Disabled = true },
        new() { Id = "flat-forced-active", Label = "Forced-active step (IsActive override)", Href = "counter", IsActive = true },
        new() { Id = "flat-thai", Label = "รายงานประจำเดือน", Href = "foundation" },
        new() { Id = "flat-long", Label = "This is an intentionally very long navigation label used to verify text truncation", Href = "sidebar-gallery" },
    };

    public static IReadOnlyList<SteconNavItem> Grouped { get; } = new List<SteconNavItem>
    {
        new() { Id = "grouped-home", Label = "Home", Href = "sidebar-gallery", Match = NavLinkMatch.All, Icon = IconName.Home, Group = "Main" },
        new() { Id = "grouped-counter", Label = "Counter", Href = "counter", Group = "Main", Badge = "New", BadgeVariant = BadgeVariant.Success },
        new() { Id = "grouped-weather", Label = "Weather", Href = "weather", Group = "Main" },
        new() { Id = "grouped-foundation", Label = "Foundation Tokens", Href = "foundation", Group = "Settings" },
        new() { Id = "grouped-restricted", Label = "Restricted area", Group = "Settings", Disabled = true },
        new() { Id = "grouped-thai", Label = "การตั้งค่าระบบ", Href = "foundation", Group = "Settings" },
    };

    public static IReadOnlyList<SteconNavItem> Nested { get; } = new List<SteconNavItem>
    {
        new() { Id = "nested-home", Label = "Home", Href = "sidebar-gallery", Match = NavLinkMatch.All, Icon = IconName.Home },
        new()
        {
            Id = "nested-demo",
            Label = "Demo pages",
            Children = new List<SteconNavItem>
            {
                new() { Id = "nested-counter", Label = "Counter", Href = "counter" },
                new() { Id = "nested-weather", Label = "Weather", Href = "weather" },
                new()
                {
                    Id = "nested-foundation-group",
                    Label = "Foundation",
                    Children = new List<SteconNavItem>
                    {
                        new() { Id = "nested-foundation", Label = "Tokens & Theme", Href = "foundation" },
                    }
                }
            }
        },
        new() { Id = "nested-thai", Label = "แผงควบคุมภาษาไทยที่ค่อนข้างยาว", Href = "sidebar-gallery" },
        new() { Id = "nested-long", Label = "Another intentionally very long navigation label for truncation checks", Href = "sidebar-gallery" },
    };
}
