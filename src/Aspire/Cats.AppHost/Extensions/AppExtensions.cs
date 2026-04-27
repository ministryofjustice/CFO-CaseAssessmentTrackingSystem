namespace Cats.AppHost.Extensions;

internal static class AppExtensions
{

    public static IDistributedApplicationBuilder AddCatsServices(this IDistributedApplicationBuilder builder,
        IResourceBuilder<RabbitMQServerResource> rabbit,
        CatsDatabaseResources databases)
    {
        builder.AddProject<Projects.Server_UI>("cats")
        .WithCatsDatabaseReference(databases.CatsDb)
        .WithReference(rabbit)
        .WaitFor(rabbit);        
        return builder;
    }

    private static IResourceBuilder<ProjectResource> WithCatsDatabaseReference(this IResourceBuilder<ProjectResource> builder, CatsDatabaseResource database)
    {
        builder.WithReference(database.DatabaseResource)
            .WaitForCompletion(database.SeedingProjectResource);
        return builder;
    }
    public static IResourceBuilder<RabbitMQServerResource> AddMessageBroker(this IDistributedApplicationBuilder builder)
    {
        var rabbit = builder.AddRabbitMQ("rabbit", port: 5672)
            // 4.3.0-management-alpine
            .WithImageSHA256("8724c6573b43d315bcec914f16a509d0b5faf5f3b281a832b484ece0bbcbe914")
            .WithDataVolume("cats-aspire-rabbit")
            .WithLifetime(ContainerLifetime.Persistent)
            .WithHttpEndpoint(port: 15672, targetPort: 15672);            

        return rabbit;

    }
}