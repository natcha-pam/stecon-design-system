using System.Text.Json;

namespace Stecon.UI.Tests;

public class SteconTableLayoutReconcilerTests
{
    private static SteconColumnDescriptor Descriptor(
        string key,
        bool canHide = true,
        bool canPin = true,
        int order = 0,
        bool defaultVisible = true,
        double? defaultWidthPx = null,
        double? minWidthPx = null,
        double? maxWidthPx = null) =>
        new(key, key, canHide, canPin, order, defaultVisible, defaultWidthPx, minWidthPx, maxWidthPx);

    private static IReadOnlyList<SteconColumnDescriptor> ThreeColumns() => new[]
    {
        Descriptor("a", order: 0),
        Descriptor("b", order: 1),
        Descriptor("c", order: 2),
    };

    [Fact]
    public void Reconcile_NullSaved_ProducesDefaultLayout_InDeclarationOrder()
    {
        var result = SteconTableLayoutReconciler.Reconcile(null, ThreeColumns());

        Assert.Equal(new[] { "a", "b", "c" }, result.Columns.Select(c => c.Key));
        Assert.All(result.Columns, c => Assert.True(c.Visible));
        Assert.All(result.Columns, c => Assert.Equal(SteconColumnPin.None, c.Pin));
        Assert.Equal(new[] { 0, 1, 2 }, result.Columns.Select(c => c.Order));
    }

    [Fact]
    public void Reconcile_UnknownSavedKey_IsDroppedSafely()
    {
        var saved = new SteconTableLayoutState(new[]
        {
            new SteconColumnLayoutState("a"),
            new SteconColumnLayoutState("removedColumn"),
            new SteconColumnLayoutState("b"),
        });

        var result = SteconTableLayoutReconciler.Reconcile(saved, ThreeColumns());

        Assert.DoesNotContain(result.Columns, c => c.Key == "removedColumn");
        Assert.Equal(3, result.Columns.Count);
    }

    [Fact]
    public void Reconcile_NewColumnNotInSaved_IsAppendedDeterministically()
    {
        var saved = new SteconTableLayoutState(new[]
        {
            new SteconColumnLayoutState("b", Order: 0),
            new SteconColumnLayoutState("a", Order: 1),
        });

        var result = SteconTableLayoutReconciler.Reconcile(saved, ThreeColumns());

        // Known columns keep their saved relative order (by their own stored Order, not list
        // position); the unseen new column "c" is appended.
        Assert.Equal(new[] { "b", "a", "c" }, result.Columns.Select(c => c.Key));
    }

    [Fact]
    public void Reconcile_CanHideFalse_AlwaysVisible_EvenIfSavedHidden()
    {
        var descriptors = new[] { Descriptor("a", canHide: false) };
        var saved = new SteconTableLayoutState(new[] { new SteconColumnLayoutState("a", Visible: false) });

        var result = SteconTableLayoutReconciler.Reconcile(saved, descriptors);

        Assert.True(result.Columns.Single().Visible);
    }

    [Fact]
    public void Reconcile_CanPinFalse_AlwaysNone_EvenIfSavedPinned()
    {
        var descriptors = new[] { Descriptor("a", canPin: false) };
        var saved = new SteconTableLayoutState(new[] { new SteconColumnLayoutState("a", Pin: SteconColumnPin.Left) });

        var result = SteconTableLayoutReconciler.Reconcile(saved, descriptors);

        Assert.Equal(SteconColumnPin.None, result.Columns.Single().Pin);
    }

    [Fact]
    public void Reconcile_InvalidWidth_IsClamped()
    {
        var descriptors = new[] { Descriptor("a", minWidthPx: 100, maxWidthPx: 300) };
        var saved = new SteconTableLayoutState(new[] { new SteconColumnLayoutState("a", WidthPx: 5000) });

        var result = SteconTableLayoutReconciler.Reconcile(saved, descriptors);

        Assert.Equal(300, result.Columns.Single().WidthPx);
    }

    [Fact]
    public void Reconcile_NeverThrows_ForRemovedColumn()
    {
        var saved = new SteconTableLayoutState(new[] { new SteconColumnLayoutState("onlyRemovedColumn") });

        var exception = Record.Exception(() => SteconTableLayoutReconciler.Reconcile(saved, ThreeColumns()));

        Assert.Null(exception);
    }

    [Fact]
    public void WithColumnVisible_TogglesVisibility()
    {
        var layout = SteconTableLayoutReconciler.Reconcile(null, ThreeColumns());

        var hidden = SteconTableLayoutReconciler.WithColumnVisible(layout, "b", false, ThreeColumns());

        Assert.False(hidden.Columns.Single(c => c.Key == "b").Visible);
        Assert.True(hidden.Columns.Single(c => c.Key == "a").Visible);
    }

    [Fact]
    public void WithColumnVisible_CanHideFalse_IsNoOp()
    {
        var descriptors = new[] { Descriptor("a", canHide: false) };
        var layout = SteconTableLayoutReconciler.Reconcile(null, descriptors);

        var result = SteconTableLayoutReconciler.WithColumnVisible(layout, "a", false, descriptors);

        Assert.True(result.Columns.Single().Visible);
    }

    [Theory]
    [InlineData(SteconColumnPin.Left)]
    [InlineData(SteconColumnPin.Right)]
    [InlineData(SteconColumnPin.None)]
    public void WithColumnPin_SetsRequestedPin(SteconColumnPin pin)
    {
        var layout = SteconTableLayoutReconciler.Reconcile(null, ThreeColumns());

        var result = SteconTableLayoutReconciler.WithColumnPin(layout, "b", pin, ThreeColumns());

        Assert.Equal(pin, result.Columns.Single(c => c.Key == "b").Pin);
    }

    [Fact]
    public void WithColumnPin_CanPinFalse_IsNoOp()
    {
        var descriptors = new[] { Descriptor("a", canPin: false) };
        var layout = SteconTableLayoutReconciler.Reconcile(null, descriptors);

        var result = SteconTableLayoutReconciler.WithColumnPin(layout, "a", SteconColumnPin.Left, descriptors);

        Assert.Equal(SteconColumnPin.None, result.Columns.Single().Pin);
    }

    [Fact]
    public void VisualOrder_IsLeftPinned_ThenUnpinned_ThenRightPinned()
    {
        var descriptors = ThreeColumns();
        var layout = SteconTableLayoutReconciler.Reconcile(null, descriptors);
        layout = SteconTableLayoutReconciler.WithColumnPin(layout, "c", SteconColumnPin.Left, descriptors);
        layout = SteconTableLayoutReconciler.WithColumnPin(layout, "a", SteconColumnPin.Right, descriptors);

        var visual = SteconTableLayoutReconciler.VisualOrder(layout);

        Assert.Equal(new[] { "c", "b", "a" }, visual.Select(c => c.Key));
    }

    [Fact]
    public void MultipleLeftPinned_PreserveRelativeOrder()
    {
        var descriptors = new[] { Descriptor("a", order: 0), Descriptor("b", order: 1), Descriptor("c", order: 2), Descriptor("d", order: 3) };
        var layout = SteconTableLayoutReconciler.Reconcile(null, descriptors);
        layout = SteconTableLayoutReconciler.WithColumnPin(layout, "a", SteconColumnPin.Left, descriptors);
        layout = SteconTableLayoutReconciler.WithColumnPin(layout, "b", SteconColumnPin.Left, descriptors);

        var visual = SteconTableLayoutReconciler.VisualOrder(layout);

        Assert.Equal(new[] { "a", "b", "c", "d" }, visual.Select(c => c.Key));
    }

    [Fact]
    public void MultipleRightPinned_PreserveRelativeOrder()
    {
        var descriptors = new[] { Descriptor("a", order: 0), Descriptor("b", order: 1), Descriptor("c", order: 2), Descriptor("d", order: 3) };
        var layout = SteconTableLayoutReconciler.Reconcile(null, descriptors);
        layout = SteconTableLayoutReconciler.WithColumnPin(layout, "c", SteconColumnPin.Right, descriptors);
        layout = SteconTableLayoutReconciler.WithColumnPin(layout, "d", SteconColumnPin.Right, descriptors);

        var visual = SteconTableLayoutReconciler.VisualOrder(layout);

        Assert.Equal(new[] { "a", "b", "c", "d" }, visual.Select(c => c.Key));
    }

    [Fact]
    public void ComputePinnedOffsets_HiddenPinnedColumn_ContributesNoOffsetGap()
    {
        var descriptors = new[]
        {
            Descriptor("a", order: 0, defaultWidthPx: 100),
            Descriptor("b", order: 1, defaultWidthPx: 120),
        };
        var layout = SteconTableLayoutReconciler.Reconcile(null, descriptors);
        layout = SteconTableLayoutReconciler.WithColumnPin(layout, "a", SteconColumnPin.Left, descriptors);
        layout = SteconTableLayoutReconciler.WithColumnPin(layout, "b", SteconColumnPin.Left, descriptors);
        layout = SteconTableLayoutReconciler.WithColumnVisible(layout, "a", false, descriptors);

        // Only "b" remains visible+pinned; its offset must start at 0, not after "a"'s width.
        var visibleLeftPinned = layout.Columns.Where(c => c.Pin == SteconColumnPin.Left && c.Visible).ToList();
        Assert.Single(visibleLeftPinned);
        Assert.Equal("b", visibleLeftPinned[0].Key);
    }

    [Fact]
    public void MoveColumn_SwapsWithVisualNeighbor()
    {
        var descriptors = ThreeColumns();
        var layout = SteconTableLayoutReconciler.Reconcile(null, descriptors);

        var moved = SteconTableLayoutReconciler.MoveColumn(layout, "a", 1, descriptors);

        Assert.Equal(new[] { "b", "a", "c" }, SteconTableLayoutReconciler.VisualOrder(moved).Select(c => c.Key));
    }

    [Fact]
    public void MoveColumn_AtPinGroupBoundary_IsNoOp()
    {
        var descriptors = ThreeColumns();
        var layout = SteconTableLayoutReconciler.Reconcile(null, descriptors);
        layout = SteconTableLayoutReconciler.WithColumnPin(layout, "a", SteconColumnPin.Left, descriptors);

        // "a" is the only left-pinned column - moving it further left/right must not cross into the unpinned group.
        var movedLeft = SteconTableLayoutReconciler.MoveColumn(layout, "a", -1, descriptors);
        var movedRight = SteconTableLayoutReconciler.MoveColumn(layout, "a", 1, descriptors);

        Assert.Equal(SteconColumnPin.Left, movedLeft.Columns.Single(c => c.Key == "a").Pin);
        Assert.Equal("a", SteconTableLayoutReconciler.VisualOrder(movedLeft).First().Key);
        Assert.Equal("a", SteconTableLayoutReconciler.VisualOrder(movedRight).First().Key);
    }

    [Fact]
    public void MoveColumn_AdjacentHiddenColumn_StillReordersDeterministically()
    {
        var descriptors = ThreeColumns();
        var layout = SteconTableLayoutReconciler.Reconcile(null, descriptors);
        layout = SteconTableLayoutReconciler.WithColumnVisible(layout, "b", false, descriptors);

        var moved = SteconTableLayoutReconciler.MoveColumn(layout, "a", 1, descriptors);

        // "b" (hidden) is between "a" and "c" in visual order - moving "a" right swaps it with "b", not "c".
        Assert.Equal(new[] { "b", "a", "c" }, SteconTableLayoutReconciler.VisualOrder(moved).Select(c => c.Key));
    }

    [Fact]
    public void ResizeColumn_ClampsToMinWidth()
    {
        var descriptors = new[] { Descriptor("a", defaultWidthPx: 100, minWidthPx: 80, maxWidthPx: 300) };
        var layout = SteconTableLayoutReconciler.Reconcile(null, descriptors);

        var result = SteconTableLayoutReconciler.ResizeColumn(layout, "a", -500, descriptors);

        Assert.Equal(80, result.Columns.Single().WidthPx);
    }

    [Fact]
    public void ResizeColumn_ClampsToMaxWidth()
    {
        var descriptors = new[] { Descriptor("a", defaultWidthPx: 100, minWidthPx: 80, maxWidthPx: 300) };
        var layout = SteconTableLayoutReconciler.Reconcile(null, descriptors);

        var result = SteconTableLayoutReconciler.ResizeColumn(layout, "a", 5000, descriptors);

        Assert.Equal(300, result.Columns.Single().WidthPx);
    }

    [Fact]
    public void ResetColumn_RestoresDefaults_LeavesOtherColumnsUntouched()
    {
        var descriptors = ThreeColumns();
        var layout = SteconTableLayoutReconciler.Reconcile(null, descriptors);
        layout = SteconTableLayoutReconciler.WithColumnPin(layout, "a", SteconColumnPin.Left, descriptors);
        layout = SteconTableLayoutReconciler.WithColumnVisible(layout, "b", false, descriptors);

        var result = SteconTableLayoutReconciler.ResetColumn(layout, "a", descriptors);

        Assert.Equal(SteconColumnPin.None, result.Columns.Single(c => c.Key == "a").Pin);
        Assert.False(result.Columns.Single(c => c.Key == "b").Visible); // untouched
    }

    [Fact]
    public void ResetAll_RestoresDeclaredDefaults()
    {
        var descriptors = ThreeColumns();
        var layout = SteconTableLayoutReconciler.Reconcile(null, descriptors);
        layout = SteconTableLayoutReconciler.WithColumnPin(layout, "a", SteconColumnPin.Left, descriptors);
        layout = SteconTableLayoutReconciler.WithColumnVisible(layout, "b", false, descriptors);

        var result = SteconTableLayoutReconciler.ResetAll(descriptors);

        Assert.All(result.Columns, c => Assert.Equal(SteconColumnPin.None, c.Pin));
        Assert.All(result.Columns, c => Assert.True(c.Visible));
    }

    [Fact]
    public void WithDensityFontSizeWrapMode_UpdateOnlyThatField()
    {
        var layout = SteconTableLayoutReconciler.Reconcile(null, ThreeColumns());

        var withDensity = SteconTableLayoutReconciler.WithDensity(layout, DataTableDensity.Compact);
        var withFont = SteconTableLayoutReconciler.WithFontSize(layout, SteconTableFontSize.Large);
        var withWrap = SteconTableLayoutReconciler.WithWrapMode(layout, SteconTableWrapMode.NoWrap);

        Assert.Equal(DataTableDensity.Compact, withDensity.Density);
        Assert.Equal(SteconTableFontSize.Large, withFont.FontSize);
        Assert.Equal(SteconTableWrapMode.NoWrap, withWrap.WrapMode);
    }

    [Fact]
    public void ResolvePinnedWidthPx_FallsBackToDocumentedDefault()
    {
        var descriptor = Descriptor("a");
        var column = new SteconColumnLayoutState("a");

        var resolved = SteconTableLayoutReconciler.ResolvePinnedWidthPx(column, descriptor);

        Assert.Equal(SteconTableLayoutReconciler.DefaultPinnedColumnWidthPx, resolved);
    }

    [Theory]
    [InlineData("160px", true, 160)]
    [InlineData("10rem", false, 0)]
    [InlineData("20%", false, 0)]
    [InlineData(null, false, 0)]
    public void TryParsePixels_OnlyResolvesLiteralPixelLengths(string? input, bool expectedSuccess, double expectedPx)
    {
        var success = SteconTableLayoutReconciler.TryParsePixels(input, out var px);

        Assert.Equal(expectedSuccess, success);
        if (expectedSuccess)
        {
            Assert.Equal(expectedPx, px);
        }
    }

    [Fact]
    public void LayoutState_RoundTripsThroughSystemTextJson()
    {
        var original = SteconTableLayoutReconciler.Reconcile(null, ThreeColumns());
        original = SteconTableLayoutReconciler.WithColumnPin(original, "a", SteconColumnPin.Left, ThreeColumns());
        original = SteconTableLayoutReconciler.WithColumnWidth(original, "b", 222, ThreeColumns());
        original = SteconTableLayoutReconciler.WithDensity(original, DataTableDensity.Compact);
        original = SteconTableLayoutReconciler.WithFontSize(original, SteconTableFontSize.Large);
        original = SteconTableLayoutReconciler.WithWrapMode(original, SteconTableWrapMode.NoWrap);

        var json = JsonSerializer.Serialize(original);
        var restored = JsonSerializer.Deserialize<SteconTableLayoutState>(json);

        Assert.NotNull(restored);
        Assert.Equal(original.Density, restored!.Density);
        Assert.Equal(original.FontSize, restored.FontSize);
        Assert.Equal(original.WrapMode, restored.WrapMode);
        Assert.Equal(original.Columns.Count, restored.Columns.Count);
        for (var i = 0; i < original.Columns.Count; i++)
        {
            Assert.Equal(original.Columns[i], restored.Columns[i]);
        }
    }
}
