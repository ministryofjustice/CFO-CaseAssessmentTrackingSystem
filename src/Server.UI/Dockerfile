FROM mcr.microsoft.com/dotnet/sdk:10.0.300@sha256:c0790639332692a0d56cdd81ed581cfd24d040d9839764c138994866df89a3b6 AS build
WORKDIR /src

# Copy solution-level config required to restore and build
COPY Directory.Build.props Directory.Packages.props NuGet.config global.json ./
COPY src/ src/

RUN dotnet restore src/Server.UI/Server.UI.csproj
RUN dotnet publish src/Server.UI/Server.UI.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:10.0.9@sha256:7644f992230d35cf230017189d4038c0ae0f7388b13f4f7ae1900a155bafb597 AS final
WORKDIR /app

COPY --from=build /app/publish ./

# Trust the Amazon RDS eu-west-2 root CAs for TLS to the RDS SQL Server (fetched at build).
ADD https://truststore.pki.rds.amazonaws.com/eu-west-2/eu-west-2-bundle.pem /usr/local/share/ca-certificates/rds-eu-west-2-bundle.crt
RUN update-ca-certificates

# Run as the image's non-root user (Cloud Platform requires non-root).
# APP_UID is a default set by the Microsoft .NET base image (uid 1654).
USER $APP_UID

EXPOSE 8080
ENTRYPOINT ["dotnet", "Cfo.Cats.Server.UI.dll"]
