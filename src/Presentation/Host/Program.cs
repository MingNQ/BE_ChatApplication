using Application;
using FluentValidation;
using Host.Controllers.Base;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Extensions;
using Infrastructure.Services.Chat;
using Logger.Logging.Serilog;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.Json.Serialization;

[assembly: ApiConventionType(typeof(ApiConventions))]
StaticLogger.EnsureInitialized();
Log.Information("Server starting up...");
try
{
    var builder = WebApplication.CreateBuilder(args);

    ConfigureServices(builder);

    var app = builder.Build();

    AppContext.SetSwitch("SqlServer.EnableLegacyTimestampBehavior", true);

    await ConfigureApplication(app, builder.Configuration, builder.Environment);
}
catch (Exception ex) when (ex.GetType() != typeof(HostAbortedException))
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Server Shutting down...");
    await Log.CloseAndFlushAsync();
}

static void ConfigureServices(WebApplicationBuilder builder)
{
    builder.RegisterSerilog();

    builder.Services.AddControllers(option =>
    {
    }).AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

    ValidatorOptions.Global.PropertyNameResolver = (_, member, _) => member?.Name.ToLowerFirstCharInvariant();

    builder.Services.AddInfrastructures(builder.Configuration);
    builder.Services.AddApplication();
    builder.Services.AddSignalR();
}

async Task ConfigureApplication(WebApplication app, ConfigurationManager configuration, IWebHostEnvironment environment)
{
    await app.Services.InitializeDatabasesAsync();
    app.UseInfrastructure(configuration, environment);
    await app.Services.InitializeCacheAsync();
    app.MapEndpoints();
    app.MapHub<ChatHub>("/hubs/chat");
    await app.RunAsync();
}