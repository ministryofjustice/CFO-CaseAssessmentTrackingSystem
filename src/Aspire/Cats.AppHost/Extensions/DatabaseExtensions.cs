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
            // 2022-CU26-ubuntu-22.04
            .WithImageSHA256("ba4c8329f48fb8f02e1416be6a930ebfd71268caee78aa985f3af4315e457c89");
    
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