namespace Stecon.UI;

/// <summary>Passed to <see cref="SteconFormField.ChildContent"/> so the wrapped control can wire up ids/aria state.</summary>
public sealed class FormFieldContext
{
    public required string ControlId { get; init; }

    public string? DescribedBy { get; init; }

    public bool IsInvalid { get; init; }

    public bool Required { get; init; }
}
