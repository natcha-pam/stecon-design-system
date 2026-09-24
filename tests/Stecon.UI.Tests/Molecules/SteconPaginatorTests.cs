using Bunit;

namespace Stecon.UI.Tests;

public class SteconPaginatorTests : BunitContext
{
    private IRenderedComponent<SteconPaginator> RenderPaginator(Action<ComponentParameterCollectionBuilder<SteconPaginator>>? configure = null, int page = 1)
    {
        return Render<SteconPaginator>(p =>
        {
            p.Add(x => x.Page, page);
            p.Add(x => x.PreviousLabel, "Previous");
            p.Add(x => x.NextLabel, "Next");
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void FirstPage_PreviousDisabled_NextEnabled_WhenTotalUnknown()
    {
        var cut = RenderPaginator();

        var buttons = cut.FindAll("button");
        Assert.True(buttons[0].HasAttribute("disabled"));
        Assert.False(buttons[1].HasAttribute("disabled"));
    }

    [Fact]
    public void LastPage_NextDisabled_WhenTotalCountKnown()
    {
        var cut = RenderPaginator(
            p => p.Add(x => x.PageSize, 25).Add(x => x.TotalCount, 100),
            page: 4);

        var buttons = cut.FindAll("button");
        Assert.True(buttons[1].HasAttribute("disabled"));
    }

    [Fact]
    public void NextClick_InvokesPageChanged()
    {
        int? newPage = null;
        var cut = RenderPaginator(p => p.Add(x => x.PageChanged, page => newPage = page));

        cut.FindAll("button")[1].Click();

        Assert.Equal(2, newPage);
    }

    [Fact]
    public void PreviousClick_InvokesPageChanged()
    {
        int? newPage = null;
        var cut = RenderPaginator(p => p.Add(x => x.PageChanged, page => newPage = page), page: 3);

        cut.FindAll("button")[0].Click();

        Assert.Equal(2, newPage);
    }

    [Fact]
    public void PageSizeOptionsNull_OmitsPageSizeSelector()
    {
        var cut = RenderPaginator();

        Assert.Empty(cut.FindAll("select"));
    }

    [Fact]
    public void PageSizeOptions_RendersSelector_AndChangingInvokesBothCallbacks()
    {
        var pageSizeChanged = -1;
        var pageChangedTo = -1;
        var cut = RenderPaginator(p => p
            .Add(x => x.PageSizeOptions, new[] { 10, 25, 50 })
            .Add(x => x.PageSizeLabel, "Rows per page")
            .Add(x => x.PageSizeChanged, size => pageSizeChanged = size)
            .Add(x => x.PageChanged, page => pageChangedTo = page));

        cut.Find("select").Change("50");

        Assert.Equal(50, pageSizeChanged);
        Assert.Equal(1, pageChangedTo);
    }

    [Fact]
    public void FormatStatus_UsedWhenSupplied()
    {
        var cut = RenderPaginator(
            p => p
                .Add(x => x.TotalCount, 50)
                .Add(x => x.PageSize, 25)
                .Add(x => x.FormatStatus, (Func<int, int?, string>)((page, total) => $"{page}/{total}")),
            page: 2);

        Assert.Equal("2/2", cut.Find(".stecon-paginator__status").TextContent);
    }

    [Fact]
    public void FormatStatus_Absent_ShowsBarePageNumber()
    {
        var cut = RenderPaginator(page: 3);

        Assert.Equal("3", cut.Find(".stecon-paginator__status").TextContent);
    }

    [Fact]
    public void ConsumerSuppliedClass_IsMergedWithInternalClasses()
    {
        var attributes = new Dictionary<string, object>
        {
            ["class"] = "my-custom-class",
            ["data-testid"] = "paginator",
        };

        var cut = RenderPaginator(p => p.Add(x => x.AdditionalAttributes, attributes));

        var nav = cut.Find("nav");
        Assert.Contains("stecon-paginator", nav.ClassList);
        Assert.Contains("my-custom-class", nav.ClassList);
        Assert.Equal("paginator", nav.GetAttribute("data-testid"));
    }
}
