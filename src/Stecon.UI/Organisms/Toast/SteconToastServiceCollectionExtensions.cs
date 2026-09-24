using Microsoft.Extensions.DependencyInjection;

namespace Stecon.UI;

/// <summary>DI registration for the STECON UI toast architecture (mirrors <see cref="SteconThemeServiceCollectionExtensions"/>).</summary>
public static class SteconToastServiceCollectionExtensions
{
    public static IServiceCollection AddSteconToast(this IServiceCollection services)
    {
        services.AddScoped<IToastService, ToastService>();
        return services;
    }
}
