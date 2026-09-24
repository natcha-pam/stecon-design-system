using Bunit;

namespace Stecon.UI.Tests;

public class SteconAppearanceSettingsTests : BunitContext
{
    private static readonly SteconAppearanceSettingsLabels Labels = new(
        ModeSectionLabel: "Mode",
        LightLabel: "Light",
        DarkLabel: "Dark",
        PaletteSectionLabel: "Palette",
        SteconPaletteLabel: "STECON",
        RomanticPaletteLabel: "Romantic",
        EveningPaletteLabel: "Evening",
        SunsetPaletteLabel: "Sunset",
        PurplePaletteLabel: "Purple",
        PeachPaletteLabel: "Peach",
        PeacockPaletteLabel: "Peacock",
        ResetLabel: "Reset");

    private IRenderedComponent<SteconAppearanceSettings> RenderSettings(
        SteconAppearanceState? appearance = null,
        Action<SteconAppearanceState>? onChanged = null)
    {
        return Render<SteconAppearanceSettings>(p => p
            .Add(x => x.Appearance, appearance ?? new SteconAppearanceState(ThemeMode.Light, SteconPalette.Stecon))
            .Add(x => x.Labels, Labels)
            .Add(x => x.AppearanceChanged, onChanged ?? (_ => { })));
    }

    [Fact]
    public void RendersAllSevenPaletteSwatches_WithCallerSuppliedNames()
    {
        var cut = RenderSettings();

        var names = cut.FindAll(".stecon-appearance-settings__swatch-name").Select(e => e.TextContent).ToList();
        Assert.Equal(new[] { "STECON", "Romantic", "Evening", "Sunset", "Purple", "Peach", "Peacock" }, names);
    }

    [Fact]
    public void SelectedPalette_HasCheckedRadio_AndSelectedClass()
    {
        var cut = RenderSettings(new SteconAppearanceState(ThemeMode.Light, SteconPalette.Purple));

        var swatches = cut.FindAll(".stecon-appearance-settings__swatch");
        var purpleSwatch = swatches.First(s => s.QuerySelector(".stecon-appearance-settings__swatch-name")!.TextContent == "Purple");
        Assert.Contains("stecon-appearance-settings__swatch--selected", purpleSwatch.ClassList);
        Assert.True(((AngleSharp.Html.Dom.IHtmlInputElement)purpleSwatch.QuerySelector("input")!).IsChecked);
    }

    [Fact]
    public void ClickingSwatch_InvokesAppearanceChanged_WithNewPalette_ModeUnchanged()
    {
        SteconAppearanceState? changed = null;
        var cut = RenderSettings(new SteconAppearanceState(ThemeMode.Dark, SteconPalette.Stecon), s => changed = s);

        var peacockInput = cut.FindAll(".stecon-appearance-settings__swatch")
            .First(s => s.QuerySelector(".stecon-appearance-settings__swatch-name")!.TextContent == "Peacock")
            .QuerySelector("input")!;
        peacockInput.Change(true);

        Assert.NotNull(changed);
        Assert.Equal(SteconPalette.Peacock, changed!.Palette);
        Assert.Equal(ThemeMode.Dark, changed.Mode); // Mode untouched by a palette change
    }

    [Fact]
    public void ChangingMode_InvokesAppearanceChanged_WithNewMode_PaletteUnchanged()
    {
        SteconAppearanceState? changed = null;
        var cut = RenderSettings(new SteconAppearanceState(ThemeMode.Light, SteconPalette.Romantic), s => changed = s);

        cut.FindAll("label.stecon-radio").First(l => l.TextContent.Contains("Dark")).QuerySelector("input")!.Change(true);

        Assert.NotNull(changed);
        Assert.Equal(ThemeMode.Dark, changed!.Mode);
        Assert.Equal(SteconPalette.Romantic, changed.Palette); // Palette untouched by a mode change
    }

    [Fact]
    public void Reset_InvokesAppearanceChanged_WithSteconPalette_ModeUnchanged()
    {
        SteconAppearanceState? changed = null;
        var cut = RenderSettings(new SteconAppearanceState(ThemeMode.Dark, SteconPalette.Peach), s => changed = s);

        cut.Find("button").Click();

        Assert.NotNull(changed);
        Assert.Equal(SteconPalette.Stecon, changed!.Palette);
        Assert.Equal(ThemeMode.Dark, changed.Mode);
    }

    [Fact]
    public void ConsumerSuppliedClass_IsMergedWithInternalClasses()
    {
        var attributes = new Dictionary<string, object> { ["class"] = "my-custom-class" };
        var cut = Render<SteconAppearanceSettings>(p => p
            .Add(x => x.Appearance, new SteconAppearanceState(ThemeMode.Light, SteconPalette.Stecon))
            .Add(x => x.Labels, Labels)
            .Add(x => x.AppearanceChanged, (SteconAppearanceState _) => { })
            .Add(x => x.AdditionalAttributes, attributes));

        var root = cut.Find(".stecon-appearance-settings");
        Assert.Contains("my-custom-class", root.ClassList);
    }
}
