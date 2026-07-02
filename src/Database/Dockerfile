FROM mcr.microsoft.com/dotnet/sdk:10.0.300@sha256:c0790639332692a0d56cdd81ed581cfd24d040d9839764c138994866df89a3b6 AS build
WORKDIR /src

# Install sqlpackage to a fixed, non-root path
RUN dotnet tool install --tool-path /opt/sqlpackage microsoft.sqlpackage --version 170.3.93

# Copy solution-level config required for build
COPY Directory.Build.props NuGet.config global.json ./

# Build SQL project to produce the DACPAC
COPY src/Database/CatsDb/ src/Database/CatsDb/
RUN dotnet build src/Database/CatsDb/CatsDb.sqlproj --configuration Release


FROM mcr.microsoft.com/dotnet/runtime:10.0.9@sha256:6a40d375e9c8432fcf4adebae05d7e0a276e9a90dd01174df6709a090771bebc AS final
WORKDIR /app

# Copy sqlpackage and make it accessible to non-root users
COPY --from=build /opt/sqlpackage /opt/sqlpackage
RUN chmod -R a+rx /opt/sqlpackage
ENV PATH="$PATH:/opt/sqlpackage"

# Trust the Amazon RDS eu-west-2 root CAs for TLS to the RDS SQL Server (fetched at build).
ADD https://truststore.pki.rds.amazonaws.com/eu-west-2/eu-west-2-bundle.pem /usr/local/share/ca-certificates/rds-eu-west-2-bundle.crt
RUN update-ca-certificates

# Copy DACPAC and entrypoint
COPY --from=build /src/src/Database/CatsDb/bin/Release/CatsDb.dacpac ./CatsDb.dacpac
COPY src/Database/entrypoint.sh ./
RUN chmod +x ./entrypoint.sh

# Run as the image's non-root user (Cloud Platform requires non-root).
# APP_UID is a default set by the Microsoft .NET base image (uid 1654).
USER $APP_UID

ENTRYPOINT ["./entrypoint.sh"]
