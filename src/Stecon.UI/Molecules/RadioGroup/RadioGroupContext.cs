namespace Stecon.UI;

/// <summary>Cascaded from <see cref="SteconRadioGroup{TValue}"/> to coordinate child <see cref="SteconRadio{TValue}"/> instances.</summary>
public sealed class RadioGroupContext<TValue>
{
    public required string Name { get; init; }

    public TValue SelectedValue { get; init; } = default!;

    public required Func<TValue, Task> SelectAsync { get; init; }

    public bool Disabled { get; init; }
}
