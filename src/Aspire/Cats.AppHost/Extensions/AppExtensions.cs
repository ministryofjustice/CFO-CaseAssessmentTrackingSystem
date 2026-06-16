namespace Cats.AppHost.Extensions;

internal static class AppExtensions
{

    public static IDistributedApplicationBuilder AddCatsServices(this IDistributedApplicationBuilder builder,
        IResourceBuilder<RabbitMQServerResource> rabbit,
        IResourceBuilder<RedisResource> redis,
        CatsDatabaseResources databases)
    {
        var useWorkerForJobs = string.Equals(builder.Configuration["Features:UseWorkerForJobs"], "true", StringComparison.OrdinalIgnoreCase);
        var useSignalRBackplane = string.Equals(builder.Configuration["Features:UseSignalRBackplane"], "true", StringComparison.OrdinalIgnoreCase);
        var replicaCount = int.TryParse(builder.Configuration["Replicas"], out var n) ? n : 1;
        
        var cats = builder.AddProject<Projects.Server_UI>("cats")
            .WithCatsDatabaseReference(databases.CatsDb)
            .WithEnvironment("Features__UseWorkerForJobs", useWorkerForJobs.ToString().ToLowerInvariant())
            .WithEnvironment("Features__UseSignalRBackplane", useSignalRBackplane.ToString().ToLowerInvariant())
            .WithReference(rabbit)
            .WithReference(redis)
            .WaitFor(rabbit)
            .WaitFor(redis);

        if(replicaCount > 1)
        {
            cats.WithReplicas(replicaCount);        
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