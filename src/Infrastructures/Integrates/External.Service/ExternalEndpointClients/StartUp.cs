using Application.Configurations;
using External.Service.ExternalEndpointClients.DelegatingRequestHandler;
using External.Service.ExternalEndpointClients.RefitInterfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Uri = System.Uri;

namespace External.Service.ExternalEndpointClients;

public static class Startup
{
    public static void AddRefitClients(this IServiceCollection services, IConfiguration config)
    {
        services.AddTransient<AddBearerTokenHeader>();

        var externalBaseUriSettings = config.GetSection(nameof(ExternalUriSettings)).Get<ExternalUriSettings>();

        services.AddRefitClient<IEmailServiceClient>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = externalBaseUriSettings?.EmailService is not null
                    ? new Uri(externalBaseUriSettings.EmailService)
                    : throw new ArgumentNullException(nameof(externalBaseUriSettings.EmailService));
            });

        services.AddRefitClient<INotificationServiceClient>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = externalBaseUriSettings?.EmailService is not null
                    ? new Uri(externalBaseUriSettings.EmailService)
                    : throw new ArgumentNullException(nameof(externalBaseUriSettings.EmailService));
            });
    }
}