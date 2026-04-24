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