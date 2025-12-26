# BE_ChatApplication

A backend .NET solution for a chat application. This repository contains multiple projects (Domain, Application, Infrastructure, Presentation, Shared, Common utilities) organized as a solution for scalable services, persistence, and hosting.

## Key Projects
- src/Presentation/Host: ASP.NET Core host and controllers.
- src/Infrastructures/Persistences/EFCore.Persistence: EF Core persistence layer and DbContext.
- src/Common/Logger: Shared Serilog logger helpers and settings.
- src/Core/Application: Application services, DTOs, and interfaces.
- src/Domain: Domain entities and value objects.
- src/Shared: Shared types, helpers, and constants.

## Requirements
- .NET 10 SDK (net10.0)
- dotnet CLI available on PATH
- (Optional) SQL Server / other DB as configured in `appsettings.*.json`

## Quickstart — Build & Run
1. Restore and build the solution:
```bash
dotnet restore
dotnet build
```

2. Run the host (from `src/Presentation/Host`):
```bash
cd src/Presentation/Host
dotnet run
```

3. Open API endpoints at the configured host URL (see `appsettings.json`).

## Configuration
- Application-wide settings are in `appsettings.json` and `appsettings.Development.json`.
- Logger configuration is read from a `LoggerSettings` section.
- Database connection strings and provider-specific settings are in the host `appsettings` files.

## Architecture

- **Style:** Clean Architecture with Domain-Driven Design influences (layered, dependency-inversion).
- **Goal:** Separation of concerns, testability, and independent deployability of infrastructure and host.

### Layers
- **Presentation:** `src/Presentation/Host` — ASP.NET Core controllers and HTTP surface.
- **Application:** `src/Core/Application` — Use-cases, DTOs, CQRS handlers, application services, interfaces.
- **Domain:** `src/Domain` — Entities, value objects, domain events, and business rules.
- **Infrastructure:** `src/Infrastructures/*` — EF Core persistence, repositories, external integrations, middleware.
- **Common/Shared:** `src/Common` and `src/Shared` — cross-cutting utilities (logging, constants, helpers).

### Key Patterns & Principles
- **Dependency Inversion:** Higher-level layers define interfaces; lower-level layers implement them. Application and Domain do not depend on Infrastructure.
- **CQRS / Mediator:** Commands/Queries handled in `Application/Cqrs` to separate reads and writes.
- **Repository + Unit of Work:** Persistence abstractions in `Infrastructure` with EF Core implementations in `EFCore.Persistence`.
- **Global Query Filters:** Reusable helper (`AppendGlobalQueryFilter`) to apply concerns like soft-delete at the model level.
- **DI-friendly Logging:** `src/Common/Logger` registers Serilog via `IServiceCollection` so the shared library doesn't require ASP.NET framework references.
- **Configuration-driven:** Settings and feature toggles (e.g., logging) are read from `appsettings.json`.

### Request / Call Flow (high level)
- HTTP request -> Controller (`Presentation`) -> Application service or CQRS handler -> Domain logic -> Repository (interface) -> EF Core implementation (`Infrastructure`) -> DB

### Where to look
- DI and startup wiring: `src/Presentation/Host/Program.cs` or `Startup.cs`.
- Use-cases and CQRS: `src/Core/Application/Cqrs`.
- Domain: `src/Domain/Entities`, `src/Domain/ValueObjects`.
- Persistence: `src/Infrastructures/Persistences/EFCore.Persistence`.
- Logging: `src/Common/Logger`.

## Logging (Serilog)
Use the `IServiceCollection`-based extension to register Serilog from configuration:

```csharp
// Program.cs (before builder.Build())
builder.Services.RegisterSerilog(builder.Configuration);
```

This registers a `Serilog.ILogger` singleton and sets `Log.Logger`. Settings are read from the `LoggerSettings` configuration section.

## EF Core: Global Query Filter Helper
A helper extension is included to append a global query filter to entities implementing a given interface:

Example usage in `OnModelCreating`:
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.AppendGlobalQueryFilter<ISoftDelete>(e => !e.IsDeleted);
}
```

## Project Structure (high level)
- Solution: `BE_ChatApplication.slnx`
- `src/` contains all projects grouped by responsibility.
- Each project targets `net10.0`.

## Tests
- If tests exist, run:
```bash
dotnet test
```

## Common Commands
- Restore: `dotnet restore`
- Build: `dotnet build`
- Run host: `dotnet run --project src/Presentation/Host`
- Test: `dotnet test`

## Troubleshooting
- Missing assembly/type errors for `Microsoft.AspNetCore.Builder`: ensure the host project targets ASP.NET and registers Serilog using the host project, or use the `IServiceCollection`-based `RegisterSerilog` in the shared logger library.
- EF Core query-filter issues: ensure your EF Core version matches the extension APIs used (`GetQueryFilter()`).

## License
Add your license here (e.g., MIT).
