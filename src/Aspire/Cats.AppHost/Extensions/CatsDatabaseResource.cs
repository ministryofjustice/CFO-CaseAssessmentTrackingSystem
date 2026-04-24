namespace Cats.AppHost.Extensions;

internal record CatsDatabaseResource(
    IResourceBuilder<SqlServerDatabaseResource> DatabaseResource,
    IResourceBuilder<SqlProjectResource> SqlProjectResource,
    IResourceBuilder<ProjectResource> SeedingProjectResource
);