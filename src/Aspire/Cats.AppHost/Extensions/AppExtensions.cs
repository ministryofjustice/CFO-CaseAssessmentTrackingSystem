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
            .WithImageSHA256("1a43764bdcf116542e7c8c794adc67c79461727da16d474e9e21483fe7f716d3")
            .WithDataVolume("cats-aspire-rabbit")
            .WithLifetime(ContainerLifetime.Persistent)
            .WithHttpEndpoint(port: 15672, targetPort: 15672);            

        return rabbit;

    }
}