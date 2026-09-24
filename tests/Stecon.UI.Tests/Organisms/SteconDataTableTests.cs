using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Stecon.UI.Tests;

public class SteconDataTableTests : BunitContext
{
    private sealed record TestItem(int Id, string Name);

    private static readonly IReadOnlyList<TestItem> SampleItems = new[]
    {
        new TestItem(1, "Alpha"),
        new TestItem(2, "Beta"),
        new TestItem(3, "Gamma"),
    };

    private static RenderFragment TwoColumns(bool nameSortable = false) => builder =>
    {
        builder.OpenComponent<SteconDataColumn<TestItem>>(0);
        builder.AddComponentParameter(1, "Key", "id");
        builder.AddComponentParameter(2, "Title", "Id");
        builder.AddComponentParameter(3, "CellTemplate", (RenderFragment<TestItem>)(item => b => b.AddContent(0, item.Id)));
        builder.CloseComponent();

        builder.OpenComponent<SteconDataColumn<TestItem>>(4);
        builder.AddComponentParameter(5, "Key", "name");
        builder.AddComponentParameter(6, "Title", "Name");
        builder.AddComponentParameter(7, "Sortable", nameSortable);
        builder.AddComponentParameter(8, "CellTemplate", (RenderFragment<TestItem>)(item => b => b.AddContent(0, item.Name)));
        builder.CloseComponent();
    };

    private static RenderFragment DuplicateKeyColumns() => builder =>
    {
        builder.OpenComponent<SteconDataColumn<TestItem>>(0);
        builder.AddComponentParameter(1, "Key", "id");
        builder.AddComponentParameter(2, "CellTemplate", (RenderFragment<TestItem>)(item => b => b.AddContent(0, item.Id)));
        builder.CloseComponent();

        builder.OpenComponent<SteconDataColumn<TestItem>>(3);
        builder.AddComponentParameter(4, "Key", "id");
        builder.AddComponentParameter(5, "CellTemplate", (RenderFragment<TestItem>)(item => b => b.AddContent(0, item.Id)));
        builder.CloseComponent();
    };

    private IRenderedComponent<SteconDataTable<TestItem>> RenderTable(
        Action<ComponentParameterCollectionBuilder<SteconDataTable<TestItem>>>? configure = null,
        RenderFragment? columns = null,
        IReadOnlyList<TestItem>? items = null)
    {
        return Render<SteconDataTable<TestItem>>(p =>
        {
            p.Add(x => x.Items, items ?? SampleItems);
            p.Add(x => x.RowKey, (Func<TestItem, object>)(i => i.Id));
            p.Add(x => x.Columns, columns ?? TwoColumns());
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersTableStructure_WithCaptionAndRows()
    {
        var cut = RenderTable(p => p.Add(x => x.Caption, "Test items"));

        Assert.NotEmpty(cut.FindAll("table"));
        Assert.Equal("Test items", cut.Find("caption").TextContent);
        Assert.NotEmpty(cut.FindAll("thead th"));
        Assert.Equal(3, cut.FindAll("tbody tr").Count);
    }

    [Fact]
    public void CaptionAbsent_DoesNotFailRendering()
    {
        var cut = RenderTable();

        Assert.Empty(cut.FindAll("caption"));
        Assert.NotEmpty(cut.FindAll("table"));
    }

    [Fact]
    public void AriaLabel_RendersOnTable()
    {
        var cut = RenderTable(p => p.Add(x => x.AriaLabel, "Approvals table"));

        Assert.Equal("Approvals table", cut.Find("table").GetAttribute("aria-label"));
    }

    [Fact]
    public void CellTemplate_ReceivesCorrectItem()
    {
        var cut = RenderTable();

        var firstRowCells = cut.FindAll("tbody tr")[0].QuerySelectorAll("td");
        Assert.Equal("1", firstRowCells[0].TextContent);
        Assert.Equal("Alpha", firstRowCells[1].TextContent);
    }

    [Fact]
    public void HeaderTemplate_OverridesTitle()
    {
        RenderFragment columns = builder =>
        {
            builder.OpenComponent<SteconDataColumn<TestItem>>(0);
            builder.AddComponentParameter(1, "Key", "id");
            builder.AddComponentParameter(2, "Title", "Id");
            builder.AddComponentParameter(3, "HeaderTemplate", (RenderFragment)(b => b.AddContent(0, "Custom Header")));
            builder.AddComponentParameter(4, "CellTemplate", (RenderFragment<TestItem>)(item => b => b.AddContent(0, item.Id)));
            builder.CloseComponent();
        };

        var cut = RenderTable(columns: columns);

        Assert.Equal("Custom Header", cut.Find("thead th").TextContent);
    }

    [Fact]
    public void DuplicateColumnKey_ThrowsInvalidOperationException()
    {
        Assert.ThrowsAny<Exception>(() => RenderTable(columns: DuplicateKeyColumns()));
    }

    [Fact]
    public void InvisibleColumn_IsOmittedFromHeaderAndCells()
    {
        RenderFragment columns = builder =>
        {
            builder.OpenComponent<SteconDataColumn<TestItem>>(0);
            builder.AddComponentParameter(1, "Key", "id");
            builder.AddComponentParameter(2, "Title", "Id");
            builder.AddComponentParameter(3, "CellTemplate", (RenderFragment<TestItem>)(item => b => b.AddContent(0, item.Id)));
            builder.CloseComponent();

            builder.OpenComponent<SteconDataColumn<TestItem>>(4);
            builder.AddComponentParameter(5, "Key", "name");
            builder.AddComponentParameter(6, "Title", "Name");
            builder.AddComponentParameter(7, "Visible", false);
            builder.AddComponentParameter(8, "CellTemplate", (RenderFragment<TestItem>)(item => b => b.AddContent(0, item.Name)));
            builder.CloseComponent();
        };

        var cut = RenderTable(columns: columns);

        Assert.Single(cut.FindAll("thead th"));
        Assert.Single(cut.FindAll("tbody tr")[0].QuerySelectorAll("td"));
    }

    [Fact]
    public void SortableHeader_RendersRealButton_AndInvokesSortChanged()
    {
        SortState? reported = null;
        var cut = RenderTable(
            p => p.Add(x => x.SortChanged, s => reported = s),
            TwoColumns(nameSortable: true));

        var headers = cut.FindAll("thead th");
        var sortButton = headers[1].QuerySelector("button");
        Assert.NotNull(sortButton);

        sortButton!.Click();

        Assert.Equal(new SortState("name", SortDirection.Ascending), reported);
    }

    [Fact]
    public void SortClick_CyclesThroughDirections()
    {
        // None -> Ascending
        SortState? sort = null;
        var cut = RenderTable(p => p.Add(x => x.SortChanged, s => sort = s), TwoColumns(nameSortable: true));
        cut.FindAll("thead th")[1].QuerySelector("button")!.Click();
        Assert.Equal(SortDirection.Ascending, sort!.Direction);

        // Ascending -> Descending
        sort = null;
        cut = RenderTable(
            p => p.Add(x => x.Sort, new SortState("name", SortDirection.Ascending)).Add(x => x.SortChanged, s => sort = s),
            TwoColumns(nameSortable: true));
        cut.FindAll("thead th")[1].QuerySelector("button")!.Click();
        Assert.Equal(SortDirection.Descending, sort!.Direction);

        // Descending -> None
        sort = new SortState("unset", SortDirection.None);
        cut = RenderTable(
            p => p.Add(x => x.Sort, new SortState("name", SortDirection.Descending)).Add(x => x.SortChanged, s => sort = s),
            TwoColumns(nameSortable: true));
        cut.FindAll("thead th")[1].QuerySelector("button")!.Click();
        Assert.Null(sort);
    }

    [Fact]
    public void AriaSort_ReflectsCurrentSortState()
    {
        var cut = RenderTable(
            p => p.Add(x => x.Sort, new SortState("name", SortDirection.Descending)),
            TwoColumns(nameSortable: true));

        var headers = cut.FindAll("thead th");
        Assert.Null(headers[0].GetAttribute("aria-sort")); // non-sortable column
        Assert.Equal("descending", headers[1].GetAttribute("aria-sort"));
    }

    [Fact]
    public void NonSortableColumn_HasNoAriaSortAttribute_AndNoButton()
    {
        var cut = RenderTable();

        var headers = cut.FindAll("thead th");
        Assert.Null(headers[0].GetAttribute("aria-sort"));
        Assert.Empty(headers[0].QuerySelectorAll("button"));
    }

    [Fact]
    public void SingleSelection_UsesRadioInputs_AndOnlyOneChecked()
    {
        IReadOnlyCollection<object>? selected = null;
        var cut = RenderTable(p => p
            .Add(x => x.SelectionMode, DataTableSelectionMode.Single)
            .Add(x => x.SelectedKeysChanged, keys => selected = keys));

        var radios = cut.FindAll("input[type=radio]");
        Assert.Equal(3, radios.Count);

        radios[1].Change(true);

        Assert.Equal(new object[] { 2 }, selected);
    }

    [Fact]
    public void MultipleSelection_UsesCheckboxes_AndAccumulatesKeys()
    {
        // First row selected -> reported keys contain just that row's key.
        IReadOnlyCollection<object>? selected = null;
        var cut = RenderTable(p => p
            .Add(x => x.SelectionMode, DataTableSelectionMode.Multiple)
            .Add(x => x.SelectedKeysChanged, keys => selected = keys));

        cut.FindAll("tbody input[type=checkbox]")[0].Change(true);
        Assert.Equal(new object[] { 1 }, selected);

        // Starting from an already-selected row, selecting a second row accumulates both keys.
        selected = null;
        cut = RenderTable(p => p
            .Add(x => x.SelectionMode, DataTableSelectionMode.Multiple)
            .Add(x => x.SelectedKeys, new object[] { 1 })
            .Add(x => x.SelectedKeysChanged, keys => selected = keys));

        cut.FindAll("tbody input[type=checkbox]")[1].Change(true);
        Assert.Equal(new object[] { 1, 2 }, selected);
    }

    [Fact]
    public void SelectAll_OnlySelectsCurrentRenderedPage()
    {
        IReadOnlyCollection<object>? selected = null;
        var cut = RenderTable(p => p
            .Add(x => x.SelectionMode, DataTableSelectionMode.Multiple)
            .Add(x => x.SelectAllLabel, "Select all")
            .Add(x => x.SelectedKeysChanged, keys => selected = keys));

        var selectAllCheckbox = cut.Find("thead input[type=checkbox]");
        selectAllCheckbox.Change(true);

        Assert.Equal(new object[] { 1, 2, 3 }, selected);
    }

    [Fact]
    public void RowAriaLabel_ProducesDistinctPerRowLabels()
    {
        var cut = RenderTable(p => p
            .Add(x => x.SelectionMode, DataTableSelectionMode.Multiple)
            .Add(x => x.RowAriaLabel, (Func<TestItem, string>)(item => $"Select {item.Name}")));

        var hiddenLabels = cut.FindAll("tbody .stecon-data-table__visually-hidden").Select(e => e.TextContent).ToList();

        Assert.Equal(new[] { "Select Alpha", "Select Beta", "Select Gamma" }, hiddenLabels);
    }

    [Fact]
    public void RowAriaLabel_Absent_FallsBackToKeyBasedLabel_NotIdentical()
    {
        var cut = RenderTable(p => p.Add(x => x.SelectionMode, DataTableSelectionMode.Multiple));

        var hiddenLabels = cut.FindAll("tbody .stecon-data-table__visually-hidden").Select(e => e.TextContent).ToList();

        Assert.Equal(3, hiddenLabels.Distinct().Count());
    }

    [Fact]
    public void IsLoading_RendersLoadingContent_NotRows()
    {
        var cut = RenderTable(p => p.Add(x => x.IsLoading, true));

        Assert.Single(cut.FindAll("tbody tr"));
        Assert.Contains("Loading", cut.Find("tbody tr").TextContent);
    }

    [Fact]
    public void EmptyItems_RendersEmptyContent()
    {
        var cut = RenderTable(items: Array.Empty<TestItem>());

        Assert.Single(cut.FindAll("tbody tr"));
        Assert.Contains("No data", cut.Find("tbody tr").TextContent);
    }

    [Fact]
    public void ErrorMessage_RendersAlert_AndNoRows()
    {
        var cut = RenderTable(p => p.Add(x => x.ErrorMessage, "Failed to load."));

        Assert.NotEmpty(cut.FindAll(".stecon-alert--danger"));
        Assert.Empty(cut.FindAll("tbody tr"));
    }

    [Fact]
    public void StickyHeader_TogglesClass()
    {
        var cut = RenderTable(p => p.Add(x => x.StickyHeader, true));

        Assert.Contains("stecon-data-table--sticky-header", cut.Find("table").ClassList);
    }

    [Fact]
    public void FixedLayout_SetsTableLayoutStyle()
    {
        var cut = RenderTable(p => p.Add(x => x.FixedLayout, true));

        Assert.Contains("table-layout: fixed", cut.Find("table").GetAttribute("style"));
    }

    [Theory]
    [InlineData(DataTableDensity.Compact, "stecon-data-table--compact")]
    [InlineData(DataTableDensity.Spacious, "stecon-data-table--spacious")]
    public void Density_TogglesClass(DataTableDensity density, string expectedClass)
    {
        var cut = RenderTable(p => p.Add(x => x.Density, density));

        Assert.Contains(expectedClass, cut.Find("table").ClassList);
    }

    [Fact]
    public void RowActions_ComposedViaCellTemplate_ReceivesItem()
    {
        RenderFragment columns = builder =>
        {
            builder.OpenComponent<SteconDataColumn<TestItem>>(0);
            builder.AddComponentParameter(1, "Key", "actions");
            builder.AddComponentParameter(2, "CellTemplate", (RenderFragment<TestItem>)(item => b =>
            {
                b.OpenElement(0, "button");
                b.AddAttribute(1, "class", "row-action");
                b.AddContent(2, $"Edit {item.Id}");
                b.CloseElement();
            }));
            builder.CloseComponent();
        };

        var cut = RenderTable(columns: columns);

        var buttons = cut.FindAll(".row-action");
        Assert.Equal(3, buttons.Count);
        Assert.Equal("Edit 1", buttons[0].TextContent);
    }

    [Fact]
    public void ConsumerSuppliedClass_IsMergedWithInternalClasses()
    {
        var attributes = new Dictionary<string, object>
        {
            ["class"] = "my-custom-class",
            ["data-testid"] = "approvals-table",
        };

        var cut = RenderTable(p => p.Add(x => x.AdditionalAttributes, attributes));

        var table = cut.Find("table");
        Assert.Contains("stecon-data-table", table.ClassList);
        Assert.Contains("my-custom-class", table.ClassList);
        Assert.Equal("approvals-table", table.GetAttribute("data-testid"));
    }

    [Fact]
    public void Table_NeverUsesRoleGrid()
    {
        var cut = RenderTable();

        Assert.Null(cut.Find("table").GetAttribute("role"));
    }

    [Fact]
    public void SelectAllLabel_NotSupplied_RendersNoHiddenLabelText_AndDoesNotCrash()
    {
        var cut = RenderTable(p => p.Add(x => x.SelectionMode, DataTableSelectionMode.Multiple));

        Assert.Null(cut.Find("thead").QuerySelector(".stecon-data-table__visually-hidden"));
    }

    [Fact]
    public void SelectAllLabel_Supplied_RendersAsHiddenAccessibleName()
    {
        var cut = RenderTable(p => p
            .Add(x => x.SelectionMode, DataTableSelectionMode.Multiple)
            .Add(x => x.SelectAllLabel, "Select all rows on this page"));

        Assert.Equal("Select all rows on this page", cut.Find("thead .stecon-data-table__visually-hidden").TextContent);
    }

    private sealed class VisibilityToggleHost : ComponentBase
    {
        public bool ShowNameColumn { get; private set; } = true;

        public void SetShowNameColumn(bool value)
        {
            ShowNameColumn = value;
            StateHasChanged();
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<SteconDataTable<TestItem>>(0);
            builder.AddComponentParameter(1, "Items", SampleItems);
            builder.AddComponentParameter(2, "RowKey", (Func<TestItem, object>)(i => i.Id));
            builder.AddComponentParameter(3, "Columns", (RenderFragment)(columnsBuilder =>
            {
                columnsBuilder.OpenComponent<SteconDataColumn<TestItem>>(0);
                columnsBuilder.AddComponentParameter(1, "Key", "id");
                columnsBuilder.AddComponentParameter(2, "Title", "Id");
                columnsBuilder.AddComponentParameter(3, "CellTemplate", (RenderFragment<TestItem>)(item => b => b.AddContent(0, item.Id)));
                columnsBuilder.CloseComponent();

                columnsBuilder.OpenComponent<SteconDataColumn<TestItem>>(4);
                columnsBuilder.AddComponentParameter(5, "Key", "name");
                columnsBuilder.AddComponentParameter(6, "Title", "Name");
                columnsBuilder.AddComponentParameter(7, "Visible", ShowNameColumn);
                columnsBuilder.AddComponentParameter(8, "CellTemplate", (RenderFragment<TestItem>)(item => b => b.AddContent(0, item.Name)));
                columnsBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        }
    }

    [Fact]
    public async Task ToggleVisible_ChangingConsumerState_ReflectsImmediately_NoStaleRender()
    {
        var hostCut = Render<VisibilityToggleHost>();

        Assert.Equal(2, hostCut.FindAll("thead th").Count);
        Assert.Equal(2, hostCut.FindAll("tbody tr")[0].QuerySelectorAll("td").Count);

        await hostCut.InvokeAsync(() => hostCut.Instance.SetShowNameColumn(false));

        Assert.Single(hostCut.FindAll("thead th"));
        Assert.Single(hostCut.FindAll("tbody tr")[0].QuerySelectorAll("td"));

        await hostCut.InvokeAsync(() => hostCut.Instance.SetShowNameColumn(true));

        Assert.Equal(2, hostCut.FindAll("thead th").Count);
        Assert.Equal(2, hostCut.FindAll("tbody tr")[0].QuerySelectorAll("td").Count);
    }

    private sealed class ConditionalColumnHost : ComponentBase
    {
        public bool ShowExtraColumn { get; private set; }

        public void SetShowExtraColumn(bool value)
        {
            ShowExtraColumn = value;
            StateHasChanged();
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<SteconDataTable<TestItem>>(0);
            builder.AddComponentParameter(1, "Items", SampleItems);
            builder.AddComponentParameter(2, "RowKey", (Func<TestItem, object>)(i => i.Id));
            builder.AddComponentParameter(3, "Columns", (RenderFragment)(columnsBuilder =>
            {
                columnsBuilder.OpenComponent<SteconDataColumn<TestItem>>(0);
                columnsBuilder.AddComponentParameter(1, "Key", "id");
                columnsBuilder.AddComponentParameter(2, "Title", "Id");
                columnsBuilder.AddComponentParameter(3, "CellTemplate", (RenderFragment<TestItem>)(item => b => b.AddContent(0, item.Id)));
                columnsBuilder.CloseComponent();

                if (ShowExtraColumn)
                {
                    columnsBuilder.OpenComponent<SteconDataColumn<TestItem>>(4);
                    columnsBuilder.AddComponentParameter(5, "Key", "name");
                    columnsBuilder.AddComponentParameter(6, "Title", "Name");
                    columnsBuilder.AddComponentParameter(7, "CellTemplate", (RenderFragment<TestItem>)(item => b => b.AddContent(0, item.Name)));
                    columnsBuilder.CloseComponent();
                }
            }));
            builder.CloseComponent();
        }
    }

    [Fact]
    public async Task ConditionalColumn_AddedThenRemoved_UpdatesDeterministically_OrderingStable()
    {
        var hostCut = Render<ConditionalColumnHost>();

        Assert.Single(hostCut.FindAll("thead th"));
        Assert.Equal("Id", hostCut.FindAll("thead th")[0].TextContent);

        await hostCut.InvokeAsync(() => hostCut.Instance.SetShowExtraColumn(true));

        var headers = hostCut.FindAll("thead th");
        Assert.Equal(2, headers.Count);
        Assert.Equal("Id", headers[0].TextContent);
        Assert.Equal("Name", headers[1].TextContent);

        await hostCut.InvokeAsync(() => hostCut.Instance.SetShowExtraColumn(false));

        Assert.Single(hostCut.FindAll("thead th"));
        Assert.Equal("Id", hostCut.FindAll("thead th")[0].TextContent);
    }
}
