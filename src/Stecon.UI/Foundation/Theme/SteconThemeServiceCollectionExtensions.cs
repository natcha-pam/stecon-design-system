using Microsoft.Extensions.DependencyInjection;

namespace Stecon.UI;

/// <summary>DI registration for the STECON UI theme architecture.</summary>
public static class SteconThemeServiceCollectionExtensions
{
    public static IServiceCollection AddSteconTheme(this IServiceCollection services)
    {
        services.AddScoped<IThemeService, ThemeService>();
        return services;
    }
}
