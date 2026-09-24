namespace Stecon.UI;

/// <summary>A single active toast. Duration null means persistent (manual dismiss only).</summary>
public sealed record ToastMessage(string Id, string Message, ToastVariant Variant, TimeSpan? Duration, bool Dismissible);
