namespace Stecon.UI;

public sealed class ToastOptions
{
    /// <summary>Null means persistent - manual dismiss only.</summary>
    public TimeSpan? Duration { get; init; } = TimeSpan.FromSeconds(5);

    public bool Dismissible { get; init; } = true;
}
