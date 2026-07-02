# Single "combined" image containing the three .NET apps that share a codebase:
#   * Server.UI (Blazor Server) — the container's default entrypoint
#   * Worker    (Quartz jobs + job-management API)
#   * DatabaseSeeding (one-off seeding console app)
#
# Each app is published into its own directory so their appsettings.json files do
# not collide. The Worker and Seeder run this same image with a command override
# (see helm_deploy/cats: worker.containerCommand and the seeder hook Job).
#
# The database migrator is intentionally NOT here — it is a DACPAC/sqlpackage image
# built from src/Database/Dockerfile.
FROM mcr.microsoft.com/dotnet/sdk:10.0.300@sha256:c0790639332692a0d56cdd81ed581cfd24d040d9839764c138994866df89a3b6 AS build
WORKDIR /src

# Copy solution-level config required to restore and build
COPY Directory.Build.props Directory.Packages.props NuGet.config global.json ./
COPY src/ src/

RUN dotnet restore src/Server.UI/Server.UI.csproj \
 && dotnet publish src/Server.UI/Server.UI.csproj --configuration Release --no-restore --output /app/ui

RUN dotnet restore src/Worker/Worker.csproj \
 && dotnet publish src/Worker/Worker.csproj --configuration Release --no-restore --output /app/worker

RUN dotnet restore src/DatabaseSeeding/DatabaseSeeding.csproj \
 && dotnet publish src/DatabaseSeeding/DatabaseSeeding.csproj --configuration Release --no-restore --output /app/seeder


FROM mcr.microsoft.com/dotnet/aspnet:10.0.9@sha256:7644f992230d35cf230017189d4038c0ae0f7388b13f4f7ae1900a155bafb597 AS final
WORKDIR /app

COPY --from=build /app/ui ./ui
COPY --from=build /app/worker ./worker
COPY --from=build /app/seeder ./seeder

# Trust the Amazon RDS eu-west-2 root CAs for TLS to the RDS SQL Server (fetched at build).
ADD https://truststore.pki.rds.amazonaws.com/eu-west-2/eu-west-2-bundle.pem /usr/local/share/ca-certificates/rds-eu-west-2-bundle.crt
RUN update-ca-certificates

# Run as the image's non-root user (Cloud Platform requires non-root).
# APP_UID is a default set by the Microsoft .NET base image (uid 1654).
USER $APP_UID

EXPOSE 8080
# Default to the Blazor Server UI. --contentRoot pins config/static-asset discovery
# to the UI's own directory (each app has its own appsettings.json).
ENTRYPOINT ["dotnet", "/app/ui/Cfo.Cats.Server.UI.dll", "--contentRoot", "/app/ui"]
