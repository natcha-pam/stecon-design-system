namespace Stecon.UI.Internal;

/// <summary>
/// Merges a component's own CSS classes with a consumer-supplied "class" value
/// from splatted AdditionalAttributes, instead of letting attribute splatting
/// silently overwrite the component's internal classes (Blazor's "last
/// attribute wins" rule for duplicate attribute names). Internal - not public API.
/// </summary>
internal static class CssClassMerge
{
    public static string Combine(string internalClasses, IReadOnlyDictionary<string, object>? additionalAttributes)
    {
        var consumerClass = GetConsumerClass(additionalAttributes);
        return string.IsNullOrEmpty(consumerClass)
            ? internalClasses
            : $"{internalClasses} {consumerClass}";
    }

    /// <summary>Returns AdditionalAttributes with "class" removed so it isn't also splatted verbatim (which would duplicate/override the merged class attribute above). Never mutates the caller's dictionary.</summary>
    public static IReadOnlyDictionary<string, object>? AttributesWithoutClass(IReadOnlyDictionary<string, object>? additionalAttributes)
    {
        if (additionalAttributes is null || !additionalAttributes.Keys.Any(k => string.Equals(k, "class", StringComparison.OrdinalIgnoreCase)))
        {
            return additionalAttributes;
        }

        return additionalAttributes
            .Where(kv => !string.Equals(kv.Key, "class", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(kv => kv.Key, kv => kv.Value);
    }

    private static string? GetConsumerClass(IReadOnlyDictionary<string, object>? additionalAttributes)
    {
        if (additionalAttributes is null)
        {
            return null;
        }

        foreach (var (key, value) in additionalAttributes)
        {
            if (string.Equals(key, "class", StringComparison.OrdinalIgnoreCase))
            {
                var text = value?.ToString();
                return string.IsNullOrWhiteSpace(text) ? null : text.Trim();
            }
        }

        return null;
    }
}
