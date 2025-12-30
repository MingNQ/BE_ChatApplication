using Application.Configurations;
using Application.Interfaces.Infrastructures.Integrates.External.Service.Email;
using External.Service.ExternalEndpointClients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace External.Service;

public static class Startup
{
    public static IServiceCollection AddExternalService(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<ExternalUriSettings>()
            .BindConfiguration(nameof(ExternalUriSettings))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHttpClient<IExternalEnpointClient, ExternalEnpointClient>();
        services.AddRefitClients(config);

        return services;
    }
}