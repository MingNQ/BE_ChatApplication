using Application.Common;
using Application.Configurations;
using EfCore.Persistence;
using EfCore.Persistence.Context;
using EfCore.Persistence.Initialization;
using External.Service;
using Infrastructure.Auth;
using Infrastructure.Caching;
using Infrastructure.Compression;
using Infrastructure.Cors;
using Infrastructure.Localization;
using Infrastructure.Middleware;
using Infrastructure.OpenApi;
using Infrastructure.SecurityHeaders;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Infrastructure.Validations;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Infrastructure;

public static class Startup
{
    public static IServiceCollection AddInfrastructures(this IServiceCollection services, IConfiguration configuration)
    {
        // Register infrastructure services here
        return services
            .AddHttpContextAccessor()
            .AddSettings()
            .AddApiVersioning(configuration)
            .AddAuth(configuration)
            .AddCaching(configuration)
            .AddUnitOfWork<ApplicationDbContext>()
            .AddCustomRepository()
            .AddCorsPolicy(configuration)
            .AddExceptionMiddleware()
            .AddBehaviours()
            .AddHealthCheck()
            .AddPoLocalization(configuration)
            .AddMediatR(Assembly.GetExecutingAssembly())
            .AddOpenApiDocumentation(configuration)
            .AddPersistence()
            .AddRequestLogging(configuration)
            .AddRouting(options => options.LowercaseUrls = true)
            .AddServices()
            .AddExternalService(configuration)
            .AddRegisterService()
            .AddCompressions();
    }

    private static IServiceCollection AddApiVersioning(this IServiceCollection services, IConfiguration cfg)
    {
        return services.AddApiVersioning(config =>
        {
            config.DefaultApiVersion = new ApiVersion(int.Parse(cfg["SwaggerSettings:Versions:MajorApiVersion"] ?? "1"), int.Parse(cfg["SwaggerSettings:Versions:MinorApiVersion"] ?? "0"));
            config.AssumeDefaultVersionWhenUnspecified = true;
            config.ReportApiVersions = true;
        });
    }

    private static IServiceCollection AddSettings(this IServiceCollection services)
    {
        services.AddOptions<MailServerSettings>()
            .BindConfiguration($"{nameof(MailServerSettings)}");

        services.AddOptions<AppSettings>()
            .BindConfiguration($"{nameof(AppSettings)}");

        services.AddOptions<VerificationSettings>()
            .BindConfiguration($"{nameof(VerificationSettings)}");

        return services;
    }

    private static IServiceCollection AddHealthCheck(this IServiceCollection services)
    {
        return services.AddHealthChecks().Services;
    }

    public static async Task InitializeDatabasesAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        // Create a new scope to retrieve scoped services
        using var scope = services.CreateScope();

        await scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>()
            .InitializeDatabasesAsync(cancellationToken);
    }

    public static async Task InitializeCacheAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder, IConfiguration config, IWebHostEnvironment environment) =>
    builder
        .UseCompressions()
        .UseRequestLocalization()
        .UseStaticFiles()
        .UseSecurityHeaders(config)
        .UseExceptionMiddleware()
        .UseRouting()
        .UseCorsPolicy()
        .UseAuthentication()
        .UseCurrentUser()
        .UseAuthorization()
        .UseRequestLogging(config)
        .UseOpenApiDocumentation(config, environment);

    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapControllers();
        builder.MapHealthCheck();
        return builder;
    }

    private static IEndpointConventionBuilder MapHealthCheck(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapHealthChecks("/api/health");
}