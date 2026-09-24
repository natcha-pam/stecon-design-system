using System.Text.Json;

namespace Stecon.UI.Tests;

public class SteconAppearanceStateTests
{
    [Fact]
    public void RoundTripsThroughSystemTextJson()
    {
        var original = new SteconAppearanceState(ThemeMode.Dark, SteconPalette.Peacock);

        var json = JsonSerializer.Serialize(original);
        var restored = JsonSerializer.Deserialize<SteconAppearanceState>(json);

        Assert.Equal(original, restored);
    }

    [Fact]
    public void DefaultPalette_IsStecon()
    {
        var state = new SteconAppearanceState(ThemeMode.Light, default);

        Assert.Equal(SteconPalette.Stecon, state.Palette);
    }

    [Fact]
    public void ContainsNoTableLayoutProperties()
    {
        // Appearance and DataTable personalization are independent - a saved appearance state
        // must never carry column/table layout information.
        var properties = typeof(SteconAppearanceState).GetProperties().Select(p => p.Name).ToArray();

        Assert.Equal(new[] { "Mode", "Palette" }, properties);
        Assert.DoesNotContain(typeof(SteconAppearanceState).GetProperties(), p => p.PropertyType == typeof(SteconTableLayoutState));
    }
}
