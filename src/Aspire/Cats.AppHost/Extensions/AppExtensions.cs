namespace Cats.AppHost.Extensions;

internal static class AppExtensions
{

    public static IDistributedApplicationBuilder AddCatsServices(this IDistributedApplicationBuilder builder,
        IResourceBuilder<RabbitMQServerResource> rabbit,
        CatsDatabaseResources databases)
    {
        var replicaCount = int.TryParse(builder.Configuration["ReplicaCount"], out var n) ? n : 2;

        var instances = Enumerable.Range(0, replicaCount)
            .Select(i => builder.AddProject<Projects.Server_UI>($"cats-{i}")
                .WithHttpsEndpoint(name: "https", port: 7060 + i, isProxied: false)
                .WithHttpEndpoint(name: "http", port: 5030 + i, isProxied: false)
                .WithCatsDatabaseReference(databases.CatsDb)
                .WithReference(rabbit)
                .WaitFor(rabbit))
            .ToList();

        if(replicaCount > 1)
        {
            var proxy = builder.AddProject<Projects.Cats_Proxy>("cats-proxy")
                .WithEnvironment("ReplicaCount", replicaCount.ToString());

            foreach (var instance in instances)
            {
                proxy.WithReference(instance).WaitFor(instance);
            }
        }

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