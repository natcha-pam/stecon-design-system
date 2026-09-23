using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Stecon.UI;

/// <summary>Shared active-route computation used by Sidebar/Item rendering and auto-expand seeding.</summary>
internal static class SteconNavActivation
{
    public static bool IsActive(SteconNavItem item, NavigationManager navigationManager)
    {
        if (item.IsActive.HasValue)
        {
            return item.IsActive.Value;
        }

        if (string.IsNullOrEmpty(item.Href))
        {
            return false;
        }

        var target = navigationManager.ToAbsoluteUri(item.Href).ToString().TrimEnd('/');
        var current = navigationManager.Uri.TrimEnd('/');

        return item.Match == NavLinkMatch.All
            ? string.Equals(current, target, StringComparison.OrdinalIgnoreCase)
            : current.StartsWith(target, StringComparison.OrdinalIgnoreCase);
    }

    public static bool ContainsActiveDescendant(SteconNavItem item, NavigationManager navigationManager)
    {
        if (item.Children is null)
        {
            return false;
        }

        foreach (var child in item.Children)
        {
            if (IsActive(child, navigationManager) || ContainsActiveDescendant(child, navigationManager))
            {
                return true;
            }
        }

        return false;
    }
}
