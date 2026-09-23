using Bunit;
using Microsoft.AspNetCore.Components;

namespace Stecon.UI.Tests;

public class SteconFormFieldTests : BunitContext
{
    [Fact]
    public void LabelAssociatesWith_GeneratedControlId()
    {
        var cut = Render<SteconFormField>(p => p
            .Add(x => x.Label, "Name")
            .Add(x => x.ChildContent, (RenderFragment<FormFieldContext>)(ctx => builder =>
            {
                builder.OpenElement(0, "input");
                builder.AddAttribute(1, "id", ctx.ControlId);
                builder.CloseElement();
            })));

        var label = cut.Find("label.stecon-form-field__label");
        var input = cut.Find("input");

        Assert.Equal(input.GetAttribute("id"), label.GetAttribute("for"));
    }

    [Fact]
    public void HelperText_IsExposedViaDescribedBy_WhenNoError()
    {
        FormFieldContext? captured = null;
        var cut = Render<SteconFormField>(p => p
            .Add(x => x.HelperText, "Helper")
            .Add(x => x.ChildContent, (RenderFragment<FormFieldContext>)(ctx =>
            {
                captured = ctx;
                return builder => { };
            })));

        var helperId = cut.Find(".stecon-form-field__helper").GetAttribute("id");

        Assert.NotNull(captured);
        Assert.False(captured!.IsInvalid);
        Assert.Equal(helperId, captured.DescribedBy);
    }

    [Fact]
    public void ErrorText_TakesPrecedenceOverHelperText()
    {
        var cut = Render<SteconFormField>(p => p
            .Add(x => x.HelperText, "Helper text")
            .Add(x => x.ErrorText, "Error text")
            .Add(x => x.ChildContent, (RenderFragment<FormFieldContext>)(ctx => builder => { })));

        Assert.Empty(cut.FindAll(".stecon-form-field__helper"));
        var error = cut.Find(".stecon-form-field__error");
        Assert.Equal("Error text", error.TextContent);
    }

    [Fact]
    public void ErrorText_MarksContextInvalid_AndDescribedByPointsToError()
    {
        FormFieldContext? captured = null;
        var cut = Render<SteconFormField>(p => p
            .Add(x => x.ErrorText, "Required")
            .Add(x => x.ChildContent, (RenderFragment<FormFieldContext>)(ctx =>
            {
                captured = ctx;
                return builder => { };
            })));

        var errorId = cut.Find(".stecon-form-field__error").GetAttribute("id");

        Assert.NotNull(captured);
        Assert.True(captured!.IsInvalid);
        Assert.Equal(errorId, captured.DescribedBy);
    }

    [Fact]
    public void Required_RendersVisibleAndVisuallyHiddenIndicators()
    {
        var cut = Render<SteconFormField>(p => p
            .Add(x => x.Label, "Name")
            .Add(x => x.Required, true)
            .Add(x => x.ChildContent, (RenderFragment<FormFieldContext>)(ctx => builder => { })));

        Assert.NotEmpty(cut.FindAll(".stecon-form-field__required"));
        Assert.NotEmpty(cut.FindAll(".stecon-visually-hidden"));
    }
}
