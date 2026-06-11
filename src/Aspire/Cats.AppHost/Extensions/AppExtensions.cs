namespace Cats.AppHost.Extensions;

internal static class AppExtensions
{

    public static IDistributedApplicationBuilder AddCatsServices(this IDistributedApplicationBuilder builder,
        IResourceBuilder<RabbitMQServerResource> rabbit,
        CatsDatabaseResources databases)
    {
        var useWorkerForJobs = string.Equals(builder.Configuration["Features:UseWorkerForJobs"], "true", StringComparison.OrdinalIgnoreCase);
        var useSignalRBackplane = string.Equals(builder.Configuration["Features:UseSignalRBackplane"], "true", StringComparison.OrdinalIgnoreCase);
        var enablePresenceHub = string.Equals(builder.Configuration["Features:PresenceHub:Enabled"], "true", StringComparison.OrdinalIgnoreCase);
        var relayUserPresenceNotifications = string.Equals(builder.Configuration["Features:PresenceHub:RelayUserPresenceNotifications"], "true", StringComparison.OrdinalIgnoreCase);
        var replicas = int.TryParse(builder.Configuration["Replicas"], out var n) ? n : 2;
        
        var cats = builder.AddProject<Projects.Server_UI>("cats")
            .WithCatsDatabaseReference(databases.CatsDb)
            .WithEnvironment("Features__UseWorkerForJobs", useWorkerForJobs.ToString().ToLowerInvariant())
            .WithEnvironment("Features__UseSignalRBackplane", useSignalRBackplane.ToString().ToLowerInvariant())
            .WithEnvironment("Features__PresenceHub__Enabled", enablePresenceHub.ToString().ToLowerInvariant())
            .WithEnvironment("Features__PresenceHub__RelayUserPresenceNotifications", relayUserPresenceNotifications.ToString().ToLowerInvariant())
            .WithReference(rabbit)
            .WaitFor(rabbit);

        var instances = Enumerable.Range(0, replicas)
            .Select(i => builder.AddProject<Projects.Server_UI>($"cats-{i}")
                .WithHttpsEndpoint(name: "https", port: 7060 + i, isProxied: false)
                .WithHttpEndpoint(name: "http", port: 5030 + i, isProxied: false)
                .WithCatsDatabaseReference(databases.CatsDb)
                .WithReference(rabbit)
                .WaitFor(rabbit))
            .ToList();

        if(replicas > 1)
        {
            var proxy = builder.AddProject<Projects.Cats_Proxy>("cats-proxy")
                .WithEnvironment("Replicas", replicas.ToString());

            foreach (var instance in instances)
            {
                proxy.WithReference(instance).WaitFor(instance);
            }
        }

        if(useSignalRBackplane)
        {
            var redis = builder.AddSignalRBackplane();
            
            cats.WithReference(redis)
                .WaitFor(redis);
        }

        if (useWorkerForJobs)
        {
            var worker = builder.AddProject<Projects.Worker>("cats-worker")
                .WithCatsDatabaseReference(databases.CatsDb)
                .WithReference(rabbit)
                .WaitFor(rabbit);

            // Give CATS a reference to the Worker so it can resolve the Worker's
            // job management API via Aspire service discovery ("https+http://cats-worker")
            cats.WithReference(worker);
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

    public static IResourceBuilder<RedisResource> AddSignalRBackplane(this IDistributedApplicationBuilder builder)
    {
        var redis = builder.AddRedis("redis")
            // 7.4-alpine
            .WithImageSHA256("b1addbe72465a718643cff9e60a58e6df1841e29d6d7d60c9a85d8d72f08d1a7")
            .WithDataVolume("cats-aspire-redis")
            .WithLifetime(ContainerLifetime.Persistent);

        return redis;
    }
}