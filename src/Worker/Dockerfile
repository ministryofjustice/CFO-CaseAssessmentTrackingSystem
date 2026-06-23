FROM mcr.microsoft.com/dotnet/sdk:10.0.300@sha256:c0790639332692a0d56cdd81ed581cfd24d040d9839764c138994866df89a3b6 AS build
WORKDIR /src

# Copy solution-level config required to restore and build
COPY Directory.Build.props Directory.Packages.props NuGet.config global.json ./
COPY src/ src/

RUN dotnet restore src/Worker/Worker.csproj
RUN dotnet publish src/Worker/Worker.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:10.0.9@sha256:ddcf70ad1ab963a4fcd41fbd722a6b660e404e87567cfbd46fd2809c21b02088 AS final
WORKDIR /app

COPY --from=build /app/publish ./
USER $APP_UID

EXPOSE 8080
ENTRYPOINT ["dotnet", "Cfo.Cats.Worker.dll"]
