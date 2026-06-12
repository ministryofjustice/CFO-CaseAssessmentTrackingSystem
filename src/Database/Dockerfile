FROM mcr.microsoft.com/dotnet/sdk:10.0@sha256:548d93f8a18a1acbe6cc127bc4f47281430d34a9e35c18afa80a8d6741c2adc3 AS build
WORKDIR /src

# Install sqlpackage to a fixed, non-root path
RUN dotnet tool install --tool-path /opt/sqlpackage microsoft.sqlpackage --version 170.3.93

# Copy solution-level config required for build
COPY Directory.Build.props NuGet.config global.json ./

# Build SQL project to produce the DACPAC
COPY src/Database/CatsDb/ src/Database/CatsDb/
RUN dotnet build src/Database/CatsDb/CatsDb.sqlproj --configuration Release


FROM mcr.microsoft.com/dotnet/runtime:10.0@sha256:58318ab0733b63d3ac0d7609c46f2718244e623a176f45991ee01fad46fbf880 AS final
WORKDIR /app

# Copy sqlpackage and make it accessible to non-root users
COPY --from=build /opt/sqlpackage /opt/sqlpackage
RUN chmod -R a+rx /opt/sqlpackage
ENV PATH="$PATH:/opt/sqlpackage"

# Copy DACPAC and entrypoint
COPY --from=build /src/src/Database/CatsDb/bin/Release/CatsDb.dacpac ./CatsDb.dacpac
COPY src/Database/entrypoint.sh ./
RUN chmod +x ./entrypoint.sh

ENTRYPOINT ["./entrypoint.sh"]
