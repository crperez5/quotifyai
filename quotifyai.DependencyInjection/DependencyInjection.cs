using Microsoft.Extensions.DependencyInjection;
using quotifyai.Infrastructure;

namespace quotifyai.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddQuotifyAiServices(this IServiceCollection services)
    {
        services
            .AddInfrastructure()
            .AddQuotesService();

        return services;
    }
}
