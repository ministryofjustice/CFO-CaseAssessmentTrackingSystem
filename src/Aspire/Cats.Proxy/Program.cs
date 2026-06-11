using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var replicaCount = builder.Configuration.GetValue("ReplicaCount", defaultValue: 2);

var destinations = Enumerable.Range(0, replicaCount)
    .ToDictionary(
        i => $"cats-{i}",
        i => new DestinationConfig { Address = $"https+http://cats-{i}" });

var routes = new[]
{
    new RouteConfig
    {
        RouteId = "cats-route",
        ClusterId = "cats-cluster",
        Match = new RouteMatch { Path = "{**catch-all}" },
        Transforms = new List<Dictionary<string, string>>
        {
            new() { ["RequestHeaderOriginalHost"] = "true" }
        }
    }
};

var clusters = new[]
{
    new ClusterConfig
    {
        ClusterId = "cats-cluster",
        LoadBalancingPolicy = "RoundRobin",
        SessionAffinity = new SessionAffinityConfig
        {
            Enabled = true,
            Policy = "Cookie",
            FailurePolicy = "Redistribute",
            AffinityKeyName = ".Cats.Affinity",
            Cookie = new SessionAffinityCookieConfig
            {
                HttpOnly = true,
                IsEssential = true,
            }
        },
        Destinations = destinations,
        HealthCheck = new HealthCheckConfig
        {
            Active = new ActiveHealthCheckConfig
            {
                Enabled = true,
                Interval = TimeSpan.FromSeconds(5),
                Timeout = TimeSpan.FromSeconds(3),
                Policy = "ConsecutiveFailures",
                Path = "/health"
            },
            Passive = new PassiveHealthCheckConfig
            {
                Enabled = true,
                Policy = "TransportFailureRate",
                ReactivationPeriod = TimeSpan.FromSeconds(30)
            }
        }
    }
};

builder.Services.AddReverseProxy()
    .LoadFromMemory(routes, clusters)
    .AddServiceDiscoveryDestinationResolver();

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapReverseProxy(pipeline => pipeline.UsePassiveHealthChecks());

app.Run();
