FROM mcr.microsoft.com/dotnet/sdk:10.0@sha256:c0790639332692a0d56cdd81ed581cfd24d040d9839764c138994866df89a3b6 AS build
WORKDIR /src

# Install sqlpackage to a fixed, non-root path
RUN dotnet tool install --tool-path /opt/sqlpackage microsoft.sqlpackage --version 170.3.93

# Copy solution-level config required for build
COPY Directory.Build.props NuGet.config global.json ./

# Build SQL project to produce the DACPAC
COPY src/Database/CatsDb/ src/Database/CatsDb/
RUN dotnet build src/Database/CatsDb/CatsDb.sqlproj --configuration Release


FROM mcr.microsoft.com/dotnet/runtime:10.0@sha256:d399699ebc8a27ab34665707ae6dc8f77ae478bd319444841a33a2b3840c5c9a AS final
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
