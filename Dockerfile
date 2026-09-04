# Single "combined" image containing the .NET apps that share a codebase:
#   * Server.UI        (Blazor Server) — the container's default entrypoint
#   * Worker           (Quartz jobs + job-management API)
#   * Consumers        (Rebus message consumers)
#   * DatabaseSeeding  (one-off seeding console app)
#   * migrate-database (one-off schema deploy — a .NET file-based app that deploys the DACPAC via DacFx)
#
# Each app is published into its own directory so their appsettings.json files do not
# collide. The Worker, Consumers, Seeder and Migrator run this same image with a command override
# (see helm_deploy/cats: worker.containerCommand and the migrator/seeder hook Jobs).
FROM mcr.microsoft.com/dotnet/sdk:10.0.400@sha256:e1ffd2a92ae84c1291bc1b6887501f8af98e6331e7af6d4c8d37168c5e87a64c AS build
WORKDIR /src

# Copy solution-level config required to restore and build
# Solution-level files
COPY Directory.Build.props Directory.Packages.props NuGet.config global.json ./

# Project files
COPY src/Aspire/Cats.ServiceDefaults/Cats.ServiceDefaults.csproj src/Aspire/Cats.ServiceDefaults/
COPY src/Application/Application.csproj src/Application/
COPY src/Domain/Domain.csproj src/Domain/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/
COPY src/Server.UI/Server.UI.csproj src/Server.UI/
COPY src/Worker/Worker.csproj src/Worker/
COPY src/Cats.Consumers/Cats.Consumers.csproj src/Cats.Consumers/
COPY src/DatabaseSeeding/DatabaseSeeding.csproj src/DatabaseSeeding/
COPY src/Database/CatsDb/CatsDb.sqlproj src/Database/CatsDb/

# Restore steps
RUN dotnet restore src/Server.UI/Server.UI.csproj 
RUN dotnet restore src/Worker/Worker.csproj
RUN dotnet restore src/Cats.Consumers/Cats.Consumers.csproj
RUN dotnet restore src/DatabaseSeeding/DatabaseSeeding.csproj
RUN dotnet restore src/Database/CatsDb/CatsDb.sqlproj

# Copy source code
COPY src/ src/
COPY scripts/ scripts/


# Build steps
RUN dotnet build src/Server.UI/Server.UI.csproj --no-restore --configuration Release
RUN dotnet build src/Worker/Worker.csproj --no-restore --configuration Release
RUN dotnet build src/Cats.Consumers/Cats.Consumers.csproj --no-restore --configuration Release
RUN dotnet build src/DatabaseSeeding/DatabaseSeeding.csproj --no-restore --configuration Release
RUN dotnet build src/Database/CatsDb/CatsDb.sqlproj --no-restore --configuration Release

# Publish steps
RUN dotnet publish src/Server.UI/Server.UI.csproj --no-build --configuration Release --output /app/ui
RUN dotnet publish src/Worker/Worker.csproj --no-build --configuration Release --output /app/worker
RUN dotnet publish src/Cats.Consumers/Cats.Consumers.csproj --no-build --configuration Release --output /app/consumers
RUN dotnet publish src/DatabaseSeeding/DatabaseSeeding.csproj --no-build --configuration Release --output /app/seeder
RUN dotnet publish src/Database/CatsDb/CatsDb.sqlproj --no-build --configuration Release --output /app/migrator/CatsDb.dacpac

# Schema-deploy tool: a .NET 10 file-based app (scripts/migrate-database.cs, no .csproj).
# publish restores its inline `#:package` (DacFx) on its own; the file sets PublishAot=false so
# it stays a normal framework-dependent app that runs on the aspnet runtime like the others.
RUN dotnet publish scripts/migrate-database.cs --configuration Release --output /app/migrator

# Trust the Amazon RDS eu-west-2 root CAs for TLS to the RDS SQL Server (fetched at build).
# This must run in the SDK stage: the chiseled final image has no shell or package manager
# (no /bin/sh, no update-ca-certificates), so the trust store is built here and copied in.
ADD https://truststore.pki.rds.amazonaws.com/eu-west-2/eu-west-2-bundle.pem /usr/local/share/ca-certificates/rds-eu-west-2-bundle.crt
RUN update-ca-certificates


FROM mcr.microsoft.com/dotnet/aspnet:10.0.11-noble-chiseled-extra@sha256:f5b3b2e2e548828d50e349726f51a5de001286f02c4bbde77db0dd34eb9f55ff AS final
WORKDIR /app

COPY --from=build /app/ui ./ui
COPY --from=build /app/worker ./worker
COPY --from=build /app/consumers ./consumers
COPY --from=build /app/seeder ./seeder
COPY --from=build /app/migrator ./migrator

# Bring across the updated CA trust store built in the SDK stage above.
COPY --from=build /etc/ssl/certs/ca-certificates.crt /etc/ssl/certs/ca-certificates.crt

# Run as the image's non-root user (Cloud Platform requires non-root).
# APP_UID is a default set by the Microsoft .NET base image (uid 1654).
USER $APP_UID

EXPOSE 8080
# Default to the Blazor Server UI. --contentRoot pins config/static-asset discovery
# to the UI's own directory (each app has its own appsettings.json).
ENTRYPOINT ["dotnet", "/app/ui/Cfo.Cats.Server.UI.dll", "--contentRoot", "/app/ui"]
