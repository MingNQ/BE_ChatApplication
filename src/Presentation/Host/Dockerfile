# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Presentation/Host/Host.csproj", "src/Presentation/Host/"]
COPY ["src/Common/Logger/Logger.csproj", "src/Common/Logger/"]
COPY ["src/Infrastructures/Infrastructure/Infrastructure.csproj", "src/Infrastructures/Infrastructure/"]
COPY ["src/Core/Application/Application.csproj", "src/Core/Application/"]
COPY ["src/Core/Domain/Domain.csproj", "src/Core/Domain/"]
COPY ["src/Core/Shared/Shared.csproj", "src/Core/Shared/"]
COPY ["src/Infrastructures/Integrates/External.Service/External.Service.csproj", "src/Infrastructures/Integrates/External.Service/"]
COPY ["src/Infrastructures/Persistences/EFCore.Persistence/EFCore.Persistence.csproj", "src/Infrastructures/Persistences/EFCore.Persistence/"]
COPY ["src/Infrastructures/Migrators/Migrators.MSSQL/Migrators.MSSQL.csproj", "src/Infrastructures/Migrators/Migrators.MSSQL/"]
RUN dotnet restore "./src/Presentation/Host/Host.csproj"
COPY . .
WORKDIR "/src/src/Presentation/Host"
RUN dotnet build "./Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Host.dll"]