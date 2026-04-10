using Projects;

namespace Cats.AppHost.Extensions;

internal static class DatabaseExtensions
{
    internal static IResourceBuilder<SqlServerServerResource> AddCatsSqlServer(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<ParameterResource> password) => builder.AddSqlServer("sql", password, 61744)
            .WithDataVolume("cats-data")
            .WithLifetime(ContainerLifetime.Persistent)
            .WithEndpointProxySupport(false)
            .WithImageTag("2022-latest");

    internal static CatsDatabaseResources AddCatsDatabases(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<SqlServerServerResource> sqlServer,
        bool seedData = false
    )
    {
        var catsDb = sqlServer.AddDatabase("CatsDb");

        var catsDbSqlProj = builder.AddSqlProject<CatsDb>("CatsSqlProj")
                                .WithReference(catsDb)
                                .WithSkipWhenDeployed();

        if (seedData)
        {
            builder.AddProject<DatabaseMigrator>("DatabaseMigrator")
                .WithReference(catsDb)
                .WaitForCompletion(catsDbSqlProj);

        }

        return new CatsDatabaseResources(new CatsDatabaseResource(catsDb, catsDbSqlProj));
    }

    
}

internal record CatsDatabaseResources(
        CatsDatabaseResource CatsDb
    );

    internal record CatsDatabaseResource(
        IResourceBuilder<SqlServerDatabaseResource> DatabaseResource,
        IResourceBuilder<SqlProjectResource> SqlProjectResource
    );