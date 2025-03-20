# Use runtime-only image (No SDK needed)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy already built files from the GitHub Actions pipeline
COPY ./publish ./

ENTRYPOINT ["dotnet", "Cfo.Cats.Server.UI.dll"]