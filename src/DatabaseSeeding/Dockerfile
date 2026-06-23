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


FROM mcr.microsoft.com/dotnet/runtime:10.0.9@sha256:58318ab0733b63d3ac0d7609c46f2718244e623a176f45991ee01fad46fbf880 AS final
WORKDIR /app

COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "DatabaseSeeding.dll"]
