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
            // 2022-CU25-ubuntu-22.04
            .WithImageSHA256("e07b9699a2b749969f19d86563ceeea22bd3a69f7f1db85a8d1ac4bdaf0c6f56");
    
    internal static CatsDatabaseResources AddCatsDatabases(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<SqlServerServerResource> sqlServer
    )
    {
        var catsDb = sqlServer.AddDatabase("CatsDb");

        var catsDbSqlProj = builder.AddSqlProject<CatsDb>("CatsSqlProj")
                                .WithReference(catsDb)
                                .WithSkipWhenDeployed();

        var seeding = builder.AddProject<DatabaseSeeding>("DatabaseSeeding")
            .WithReference(catsDb)
            .WaitForCompletion(catsDbSqlProj);

        return new CatsDatabaseResources(new CatsDatabaseResource(catsDb, catsDbSqlProj, seeding));
    }
}