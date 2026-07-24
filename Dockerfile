# Single "combined" image containing the .NET apps that share a codebase:
#   * Server.UI        (Blazor Server) — the container's default entrypoint
#   * Worker           (Quartz jobs + job-management API)
#   * DatabaseSeeding  (one-off seeding console app)
#   * migrate-database (one-off schema deploy — a .NET file-based app that deploys the DACPAC via DacFx)
#
# Each app is published into its own directory so their appsettings.json files do not
# collide. The Worker, Seeder and Migrator run this same image with a command override
# (see helm_deploy/cats: worker.containerCommand and the migrator/seeder hook Jobs).
FROM mcr.microsoft.com/dotnet/sdk:10.0.302@sha256:ed034a8bf0b24ded0cbbac07e17825d8e9ebfe21e308191d0f7421eaf5ad4664 AS build
WORKDIR /src

# Copy solution-level config required to restore and build
COPY Directory.Build.props Directory.Packages.props NuGet.config global.json ./
COPY src/ src/
COPY scripts/ scripts/

RUN dotnet restore src/Server.UI/Server.UI.csproj \
 && dotnet publish src/Server.UI/Server.UI.csproj --configuration Release --no-restore --output /app/ui

RUN dotnet restore src/Worker/Worker.csproj \
 && dotnet publish src/Worker/Worker.csproj --configuration Release --no-restore --output /app/worker

RUN dotnet restore src/DatabaseSeeding/DatabaseSeeding.csproj \
 && dotnet publish src/DatabaseSeeding/DatabaseSeeding.csproj --configuration Release --no-restore --output /app/seeder

# Schema-deploy tool: a .NET 10 file-based app (scripts/migrate-database.cs, no .csproj).
# publish restores its inline `#:package` (DacFx) on its own; the file sets PublishAot=false so
# it stays a normal framework-dependent app that runs on the aspnet runtime like the others.
RUN dotnet publish scripts/migrate-database.cs --configuration Release --output /app/migrator

# Build the SQL project to produce the DACPAC and ship it alongside the migrator, which
# deploys it to the database at release time (replaces the old standalone sqlpackage image).
RUN dotnet build src/Database/CatsDb/CatsDb.sqlproj --configuration Release \
 && cp src/Database/CatsDb/bin/Release/CatsDb.dacpac /app/migrator/CatsDb.dacpac

# Trust the Amazon RDS eu-west-2 root CAs for TLS to the RDS SQL Server (fetched at build).
# This must run in the SDK stage: the chiseled final image has no shell or package manager
# (no /bin/sh, no update-ca-certificates), so the trust store is built here and copied in.
ADD https://truststore.pki.rds.amazonaws.com/eu-west-2/eu-west-2-bundle.pem /usr/local/share/ca-certificates/rds-eu-west-2-bundle.crt
RUN update-ca-certificates


FROM mcr.microsoft.com/dotnet/aspnet:10.0.10-noble-chiseled-extra@sha256:f9bd6be9b5ab75b8196bff0f0972580edaea7fa8ca04e6ef530950e33caee5b0 AS final
WORKDIR /app

# the shared hmpps-github-actions build_docker action always passes GIT_REF/GIT_BRANCH/BUILD_NUMBER as
# build args (see .github/workflows/pipeline.yml / build_app job)
ARG BUILD_NUMBER
ARG GIT_REF
ARG GIT_BRANCH

ENV BUILD_NUMBER=$BUILD_NUMBER
ENV GIT_REF=$GIT_REF
ENV GIT_BRANCH=$GIT_BRANCH

COPY --from=build /app/ui ./ui
COPY --from=build /app/worker ./worker
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
