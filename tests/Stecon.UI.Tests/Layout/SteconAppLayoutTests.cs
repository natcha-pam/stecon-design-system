using Bunit;
using Microsoft.AspNetCore.Components;

namespace Stecon.UI.Tests;

public class SteconAppLayoutTests : BunitContext
{
    [Fact]
    public void RendersSidebarHeaderAndChildContent()
    {
        var cut = Render<SteconAppLayout>(p => p
            .Add(x => x.Sidebar, (RenderFragment)(b => b.AddMarkupContent(0, "<div class='fake-sidebar'>Sidebar</div>")))
            .Add(x => x.Header, (RenderFragment)(b => b.AddMarkupContent(0, "<div class='fake-header'>Header</div>")))
            .AddChildContent("<p>Body content</p>"));

        Assert.NotEmpty(cut.FindAll(".fake-sidebar"));
        Assert.NotEmpty(cut.FindAll(".fake-header"));
        Assert.Contains("Body content", cut.Markup);
    }

    [Fact]
    public void HeaderAndSidebar_AreOptional()
    {
        var cut = Render<SteconAppLayout>(p => p.AddChildContent("<p>Body</p>"));

        Assert.Empty(cut.FindAll("header"));
        Assert.Empty(cut.FindAll(".stecon-app-layout__sidebar"));
    }

    [Fact]
    public void MainContent_IsInert_WhenMobileSidebarOpen()
    {
        var cut = Render<SteconAppLayout>(p => p
            .Add(x => x.IsMobileSidebarOpen, true)
            .AddChildContent("<p>Body</p>"));

        Assert.True(cut.Find(".stecon-app-layout__main").HasAttribute("inert"));
    }

    [Fact]
    public void MainContent_IsNotInert_WhenMobileSidebarClosed()
    {
        var cut = Render<SteconAppLayout>(p => p
            .Add(x => x.IsMobileSidebarOpen, false)
            .AddChildContent("<p>Body</p>"));

        Assert.False(cut.Find(".stecon-app-layout__main").HasAttribute("inert"));
    }
}
