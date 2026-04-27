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
            // 2022-CU24-ubuntu-22.04
            .WithImageSHA256("49b45a911dc535e9345fbfd7101a1bd8a1e190a5f29b877ef75387a061e5fcf0");
    
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