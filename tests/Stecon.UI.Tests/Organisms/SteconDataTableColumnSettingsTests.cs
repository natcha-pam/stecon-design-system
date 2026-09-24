using Bunit;

namespace Stecon.UI.Tests;

public class SteconDataTableColumnSettingsTests : BunitContext
{
    private static readonly SteconDataTableSettingsLabels Labels = new(
        ColumnsSectionLabel: "Columns",
        TableSectionLabel: "Table",
        VisibleLabel: "Visible",
        PinNoneLabel: "No pin",
        PinLeftLabel: "Pin left",
        PinRightLabel: "Pin right",
        MoveLeftLabel: "Move left",
        MoveRightLabel: "Move right",
        WidenLabel: "Widen",
        NarrowLabel: "Narrow",
        ResetColumnLabel: "Reset column",
        DensityCompactLabel: "Compact",
        DensityComfortableLabel: "Comfortable",
        DensitySpaciousLabel: "Spacious",
        FontSizeSmallLabel: "Small",
        FontSizeDefaultLabel: "Default",
        FontSizeLargeLabel: "Large",
        WrapLabel: "Wrap",
        NoWrapLabel: "No wrap",
        ResetAllLabel: "Reset all");

    private static IReadOnlyList<SteconColumnDescriptor> TwoDescriptors() => new[]
    {
        new SteconColumnDescriptor("id", "Id", true, true, 0, true, null),
        new SteconColumnDescriptor("name", "Name", true, true, 1, true, null),
    };

    private IRenderedComponent<SteconDataTableColumnSettings> RenderSettings(
        SteconTableLayoutState? layout = null,
        IReadOnlyList<SteconColumnDescriptor>? descriptors = null,
        Action<SteconTableLayoutState>? onChanged = null)
    {
        var columns = descriptors ?? TwoDescriptors();
        var state = layout ?? SteconTableLayoutReconciler.Reconcile(null, columns);

        return Render<SteconDataTableColumnSettings>(p => p
            .Add(x => x.Columns, columns)
            .Add(x => x.Layout, state)
            .Add(x => x.Labels, Labels)
            .Add(x => x.LayoutChanged, onChanged ?? (_ => { })));
    }

    [Fact]
    public void RendersOneRowPerColumn_WithTitleAsLabel()
    {
        var cut = RenderSettings();

        var items = cut.FindAll(".stecon-data-table-settings__item");
        Assert.Equal(2, items.Count);
        Assert.Contains("Id", items[0].TextContent);
    }

    [Fact]
    public void TogglingVisibleCheckbox_InvokesLayoutChanged_WithColumnHidden()
    {
        SteconTableLayoutState? changed = null;
        var cut = RenderSettings(onChanged: s => changed = s);

        cut.Find(".stecon-data-table-settings__item input[type=checkbox]").Change(false);

        Assert.NotNull(changed);
        Assert.False(changed!.Columns.Single(c => c.Key == "id").Visible);
    }

    [Fact]
    public void NonHideableColumn_CheckboxIsDisabled()
    {
        var descriptors = new[] { new SteconColumnDescriptor("id", "Id", false, true, 0, true, null) };
        var cut = RenderSettings(descriptors: descriptors);

        Assert.True(((AngleSharp.Html.Dom.IHtmlInputElement)cut.Find(".stecon-data-table-settings__item input[type=checkbox]")).IsDisabled);
    }

    [Fact]
    public void ChangingPinSelect_InvokesLayoutChanged_WithNewPin()
    {
        SteconTableLayoutState? changed = null;
        var cut = RenderSettings(onChanged: s => changed = s);

        cut.FindAll(".stecon-data-table-settings__item select")[0].Change(nameof(SteconColumnPin.Left));

        Assert.NotNull(changed);
        Assert.Equal(SteconColumnPin.Left, changed!.Columns.Single(c => c.Key == "id").Pin);
    }

    [Fact]
    public void NonPinnableColumn_PinSelectIsDisabled()
    {
        var descriptors = new[] { new SteconColumnDescriptor("id", "Id", true, false, 0, true, null) };
        var cut = RenderSettings(descriptors: descriptors);

        Assert.True(((AngleSharp.Html.Dom.IHtmlSelectElement)cut.FindAll(".stecon-data-table-settings__item select")[0]).IsDisabled);
    }

    [Fact]
    public void MoveRight_InvokesLayoutChanged_WithSwappedOrder()
    {
        SteconTableLayoutState? changed = null;
        var cut = RenderSettings(onChanged: s => changed = s);

        var moveRightButton = cut.FindAll(".stecon-data-table-settings__item button").First(b => b.TextContent.Contains("Move right"));
        moveRightButton.Click();

        Assert.NotNull(changed);
        Assert.Equal(new[] { "name", "id" }, SteconTableLayoutReconciler.VisualOrder(changed!).Select(c => c.Key));
    }

    [Fact]
    public void ResetAll_InvokesLayoutChanged_WithDeclaredDefaults()
    {
        var descriptors = TwoDescriptors();
        var layout = SteconTableLayoutReconciler.Reconcile(null, descriptors);
        layout = SteconTableLayoutReconciler.WithColumnPin(layout, "id", SteconColumnPin.Left, descriptors);

        SteconTableLayoutState? changed = null;
        var cut = RenderSettings(layout: layout, descriptors: descriptors, onChanged: s => changed = s);

        cut.FindAll("button").First(b => b.TextContent.Contains("Reset all")).Click();

        Assert.NotNull(changed);
        Assert.All(changed!.Columns, c => Assert.Equal(SteconColumnPin.None, c.Pin));
    }
}
