using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace Stecon.UI.Tests;

public class SteconSidebarTests : BunitContext
{
    public SteconSidebarTests()
    {
        Services.AddSteconTheme();
    }

    private static List<SteconNavItem> BasicItems() =>
    [
        new() { Id = "home", Label = "Home", Href = "", Match = NavLinkMatch.All },
        new() { Id = "settings", Label = "Settings", Href = "settings" },
    ];

    private BunitNavigationManager Navigation => (BunitNavigationManager)Services.GetRequiredService<NavigationManager>();

    [Fact]
    public void RendersSuppliedNavigationItems()
    {
        var cut = Render<SteconSidebar>(p => p.Add(x => x.Items, BasicItems()));

        Assert.Equal(2, cut.FindAll("li.stecon-sidebar-item").Count);
        Assert.Contains("Home", cut.Markup);
        Assert.Contains("Settings", cut.Markup);
    }

    [Fact]
    public void PlainDestinationItem_WithoutConfiguredIcon_RendersNoIconGlyphOrChevron()
    {
        // A leaf item with Href and no Children must never show an expand
        // chevron, and must not fabricate an icon when none is configured.
        var items = new List<SteconNavItem> { new() { Id = "a", Label = "A", Href = "a" } };

        var cut = Render<SteconSidebar>(p => p.Add(x => x.Items, items));

        var iconSlot = cut.Find(".stecon-sidebar-item__icon");
        Assert.Empty(iconSlot.QuerySelectorAll("svg"));
        Assert.Empty(cut.FindAll(".stecon-sidebar-item__chevron"));
    }

    [Fact]
    public void ItemRow_AlwaysCarriesTitleTooltip_RegardlessOfCollapsedFlag()
    {
        // Title must not depend solely on the bound Collapsed flag, since the
        // tablet range visually hides labels via CSS only, without touching
        // Collapsed (see SteconSidebar.razor.css tablet media query).
        var cut = Render<SteconSidebar>(p => p
            .Add(x => x.Items, BasicItems())
            .Add(x => x.Collapsed, false));

        Assert.Equal("Settings", cut.Find("a[href='settings']").GetAttribute("title"));
    }

    [Fact]
    public void AutomaticallyMarksItemActive_FromCurrentRoute()
    {
        Navigation.NavigateTo("settings");

        var cut = Render<SteconSidebar>(p => p.Add(x => x.Items, BasicItems()));

        Assert.Equal("page", cut.Find("a[href='settings']").GetAttribute("aria-current"));
        Assert.Null(cut.Find("a[href='']").GetAttribute("aria-current"));
    }

    [Fact]
    public void ExplicitIsActiveOverride_TakesPrecedenceOverRoute()
    {
        var items = new List<SteconNavItem> { new() { Id = "a", Label = "A", Href = "a", IsActive = true } };

        var cut = Render<SteconSidebar>(p => p.Add(x => x.Items, items));

        Assert.Equal("page", cut.Find("a[href='a']").GetAttribute("aria-current"));
    }

    [Fact]
    public void CollapsedToggle_FiresCollapsedChanged()
    {
        bool? changedTo = null;
        var cut = Render<SteconSidebar>(p => p
            .Add(x => x.Items, BasicItems())
            .Add(x => x.Collapsed, false)
            .Add(x => x.CollapsedChanged, v => changedTo = v));

        cut.Find("button.stecon-sidebar__collapse-toggle").Click();

        Assert.True(changedTo);
    }

    [Fact]
    public void NestedVariant_TogglesChildrenVisibility_WithAriaExpanded()
    {
        var items = new List<SteconNavItem>
        {
            new()
            {
                Id = "parent",
                Label = "Parent",
                Children = [new() { Id = "child", Label = "Child", Href = "child" }]
            }
        };

        var cut = Render<SteconSidebar>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.Variant, SidebarVariant.Nested));

        var toggle = cut.Find("button.stecon-sidebar-item__toggle");
        Assert.Equal("false", toggle.GetAttribute("aria-expanded"));
        Assert.DoesNotContain("Child", cut.Markup);

        toggle.Click();

        Assert.Equal("true", cut.Find("button.stecon-sidebar-item__toggle").GetAttribute("aria-expanded"));
        Assert.Contains("Child", cut.Markup);
    }

    [Fact]
    public void NestedVariant_AutoExpandsBranchContainingActiveItem()
    {
        Navigation.NavigateTo("child");

        var items = new List<SteconNavItem>
        {
            new()
            {
                Id = "parent",
                Label = "Parent",
                Children = [new() { Id = "child", Label = "Child", Href = "child" }]
            }
        };

        var cut = Render<SteconSidebar>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.Variant, SidebarVariant.Nested));

        Assert.Contains("Child", cut.Markup);
        Assert.Equal("true", cut.Find("button.stecon-sidebar-item__toggle").GetAttribute("aria-expanded"));
    }

    [Fact]
    public void OtherVariants_IgnoreChildren_EvenWhenSupplied()
    {
        var items = new List<SteconNavItem>
        {
            new()
            {
                Id = "parent",
                Label = "Parent",
                Href = "parent",
                Children = [new() { Id = "child", Label = "Child", Href = "child" }]
            }
        };

        var cut = Render<SteconSidebar>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.Variant, SidebarVariant.Workflow));

        Assert.Empty(cut.FindAll("button.stecon-sidebar-item__toggle"));
        Assert.DoesNotContain("Child", cut.Markup);
    }

    [Fact]
    public void GroupedVariant_RendersGroupHeaders()
    {
        var items = new List<SteconNavItem>
        {
            new() { Id = "a", Label = "A", Href = "a", Group = "Main" },
            new() { Id = "b", Label = "B", Href = "b", Group = "Settings" },
        };

        var cut = Render<SteconSidebar>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.Variant, SidebarVariant.Grouped));

        Assert.Equal(2, cut.FindAll(".stecon-sidebar-group__label").Count);
        Assert.Contains("Main", cut.Markup);
        Assert.Contains("Settings", cut.Markup);
    }

    [Fact]
    public void RendersBadge_WhenSupplied()
    {
        var items = new List<SteconNavItem>
        {
            new() { Id = "a", Label = "A", Href = "a", Badge = "5", BadgeVariant = BadgeVariant.Danger }
        };

        var cut = Render<SteconSidebar>(p => p.Add(x => x.Items, items));

        var badge = cut.Find(".stecon-badge--danger");
        Assert.Equal("5", badge.TextContent.Trim());
    }

    [Fact]
    public void DisabledItem_RendersNonInteractive_WithAriaDisabled()
    {
        var items = new List<SteconNavItem> { new() { Id = "a", Label = "A", Href = "a", Disabled = true } };

        var cut = Render<SteconSidebar>(p => p.Add(x => x.Items, items));

        var span = cut.Find("span.stecon-sidebar-item__link--disabled");
        Assert.Equal("true", span.GetAttribute("aria-disabled"));
        Assert.Empty(cut.FindAll("a[href='a']"));
    }

    [Theory]
    [InlineData(SidebarVariant.Workflow)]
    [InlineData(SidebarVariant.Compact)]
    [InlineData(SidebarVariant.Grouped)]
    [InlineData(SidebarVariant.Nested)]
    [InlineData(SidebarVariant.Floating)]
    [InlineData(SidebarVariant.Minimal)]
    public void RootElement_CarriesDistinctVariantClass(SidebarVariant variant)
    {
        var cut = Render<SteconSidebar>(p => p
            .Add(x => x.Items, BasicItems())
            .Add(x => x.Variant, variant));

        var nav = cut.Find("nav.stecon-sidebar");
        Assert.Contains($"stecon-sidebar--{variant.ToString().ToLowerInvariant()}", nav.ClassList);
    }

    [Fact]
    public void Brand_RendersLogoAndTitle_WhenSupplied()
    {
        var cut = Render<SteconSidebar>(p => p
            .Add(x => x.Items, BasicItems())
            .Add(x => x.BrandLogo, "logo.png")
            .Add(x => x.BrandTitle, "STECON"));

        Assert.Contains("logo.png", cut.Markup);
        Assert.Contains("STECON", cut.Markup);
    }

    [Fact]
    public void Brand_Omitted_WhenNotSupplied()
    {
        var cut = Render<SteconSidebar>(p => p.Add(x => x.Items, BasicItems()));

        Assert.Empty(cut.FindAll(".stecon-sidebar-brand"));
    }

    [Fact]
    public void BrandContent_OverridesLogoAndTitle()
    {
        var cut = Render<SteconSidebar>(p => p
            .Add(x => x.Items, BasicItems())
            .Add(x => x.BrandLogo, "logo.png")
            .Add(x => x.BrandContent, (RenderFragment)(b => b.AddMarkupContent(0, "<span>Custom Brand</span>"))));

        Assert.Contains("Custom Brand", cut.Markup);
        Assert.DoesNotContain("logo.png", cut.Markup);
    }

    [Fact]
    public void UserContent_RendersWhenSupplied_OmittedOtherwise()
    {
        var withUser = Render<SteconSidebar>(p => p
            .Add(x => x.Items, BasicItems())
            .Add(x => x.UserContent, (RenderFragment)(b => b.AddMarkupContent(0, "<span>Jane Doe</span>"))));
        Assert.Contains("Jane Doe", withUser.Markup);
        Assert.NotEmpty(withUser.FindAll(".stecon-sidebar-user"));

        var withoutUser = Render<SteconSidebar>(p => p.Add(x => x.Items, BasicItems()));
        Assert.Empty(withoutUser.FindAll(".stecon-sidebar-user"));
    }

    [Fact]
    public void RendersInsideDarkThemeProvider_WithoutError()
    {
        var cut = Render<SteconThemeProvider>(p => p
            .Add(x => x.InitialTheme, ThemeMode.Dark)
            .AddChildContent<SteconSidebar>(child => child.Add(x => x.Items, BasicItems())));

        Assert.Equal("dark", cut.Find("[data-theme]").GetAttribute("data-theme"));
        Assert.NotEmpty(cut.FindAll(".stecon-sidebar"));
    }

    [Fact]
    public void MobileOpen_AppliesMobileOpenClass_AndRendersBackdrop()
    {
        var cut = Render<SteconSidebar>(p => p
            .Add(x => x.Items, BasicItems())
            .Add(x => x.IsMobileOpen, true));

        Assert.Contains("stecon-sidebar--mobile-open", cut.Find("nav.stecon-sidebar").ClassList);
        Assert.NotEmpty(cut.FindAll(".stecon-sidebar-backdrop"));
    }

    [Fact]
    public void MobileClosed_RendersNoBackdrop()
    {
        var cut = Render<SteconSidebar>(p => p
            .Add(x => x.Items, BasicItems())
            .Add(x => x.IsMobileOpen, false));

        Assert.Empty(cut.FindAll(".stecon-sidebar-backdrop"));
    }

    [Fact]
    public void BackdropClick_ClosesDrawer()
    {
        bool? closedTo = null;
        var cut = Render<SteconSidebar>(p => p
            .Add(x => x.Items, BasicItems())
            .Add(x => x.IsMobileOpen, true)
            .Add(x => x.IsMobileOpenChanged, v => closedTo = v));

        cut.Find(".stecon-sidebar-backdrop").Click();

        Assert.False(closedTo);
    }

    [Fact]
    public void EscapeKey_ClosesDrawer_WhenOpen()
    {
        bool? closedTo = null;
        var cut = Render<SteconSidebar>(p => p
            .Add(x => x.Items, BasicItems())
            .Add(x => x.IsMobileOpen, true)
            .Add(x => x.IsMobileOpenChanged, v => closedTo = v));

        cut.Find("nav.stecon-sidebar").KeyDown(new KeyboardEventArgs { Key = "Escape" });

        Assert.False(closedTo);
    }

    [Fact]
    public void SelectingDestination_ClosesMobileDrawer()
    {
        bool? closedTo = null;
        var cut = Render<SteconSidebar>(p => p
            .Add(x => x.Items, BasicItems())
            .Add(x => x.IsMobileOpen, true)
            .Add(x => x.IsMobileOpenChanged, v => closedTo = v));

        cut.Find("a[href='settings']").Click();

        Assert.False(closedTo);
    }

    [Fact]
    public void ExpandingParent_DoesNotCloseMobileDrawer()
    {
        var closeInvoked = false;
        var items = new List<SteconNavItem>
        {
            new()
            {
                Id = "parent",
                Label = "Parent",
                Children = [new() { Id = "child", Label = "Child", Href = "child" }]
            }
        };

        var cut = Render<SteconSidebar>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.Variant, SidebarVariant.Nested)
            .Add(x => x.IsMobileOpen, true)
            .Add(x => x.IsMobileOpenChanged, _ => closeInvoked = true));

        cut.Find("button.stecon-sidebar-item__toggle").Click();

        Assert.False(closeInvoked);
    }

    [Fact]
    public void SameNavigationModel_DrivesDesktopAndMobilePresentation_WithoutDuplication()
    {
        var items = BasicItems();

        var desktop = Render<SteconSidebar>(p => p.Add(x => x.Items, items).Add(x => x.IsMobileOpen, false));
        var mobileOpen = Render<SteconSidebar>(p => p.Add(x => x.Items, items).Add(x => x.IsMobileOpen, true));

        Assert.Equal(2, desktop.FindAll("li.stecon-sidebar-item").Count);
        Assert.Equal(2, mobileOpen.FindAll("li.stecon-sidebar-item").Count);
    }

    [Fact]
    public void NavLandmark_UsesSuppliedAriaLabel()
    {
        var cut = Render<SteconSidebar>(p => p
            .Add(x => x.Items, BasicItems())
            .Add(x => x.AriaLabel, "Main navigation"));

        Assert.Equal("Main navigation", cut.Find("nav.stecon-sidebar").GetAttribute("aria-label"));
    }
}
