# Use runtime-only image (No SDK needed)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

# Create a non-root user with a specific UID and GID
RUN addgroup --system --gid 1001 appgroup && \
    adduser --system --uid 1001 --ingroup appgroup appuser

WORKDIR /app

# Copy already built files from the GitHub Actions pipeline
COPY ./publish ./ 

# Set ownership and permissions
RUN chown -R 1001:1001 /app

# Switch to the non-root user using the UID
USER 1001

ENTRYPOINT ["dotnet", "Cfo.Cats.Server.UI.dll"]
