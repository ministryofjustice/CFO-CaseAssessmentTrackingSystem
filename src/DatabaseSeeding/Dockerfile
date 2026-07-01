FROM mcr.microsoft.com/dotnet/sdk:10.0.300@sha256:c0790639332692a0d56cdd81ed581cfd24d040d9839764c138994866df89a3b6 AS build
WORKDIR /src

# Copy solution-level config required to restore and build
COPY Directory.Build.props Directory.Packages.props NuGet.config global.json ./
COPY src/ src/

RUN dotnet restore src/DatabaseSeeding/DatabaseSeeding.csproj
RUN dotnet publish src/DatabaseSeeding/DatabaseSeeding.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish


FROM mcr.microsoft.com/dotnet/runtime:10.0.9@sha256:6a40d375e9c8432fcf4adebae05d7e0a276e9a90dd01174df6709a090771bebc AS final
WORKDIR /app

COPY --from=build /app/publish ./

# Trust the Amazon RDS eu-west-2 root CAs for TLS to the RDS SQL Server (fetched at build).
ADD https://truststore.pki.rds.amazonaws.com/eu-west-2/eu-west-2-bundle.pem /usr/local/share/ca-certificates/rds-eu-west-2-bundle.crt
RUN update-ca-certificates

ENTRYPOINT ["dotnet", "DatabaseSeeding.dll"]
