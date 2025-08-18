var builder = DistributedApplication.CreateBuilder(args);

var sqlPassword = builder.AddParameter("sqlPassword", secret: true);
var rabbitUser = builder.AddParameter("rabbitUser", secret: true);
var rabbitPassword = builder.AddParameter("rabbitPassword", secret: true);

var k8s = builder.AddKubernetesEnvironment("k8s");

bool publishing = builder.ExecutionContext.IsPublishMode;

#pragma warning disable ASPIREPROXYENDPOINTS001
var sql = builder.AddSqlServer("sql", sqlPassword, 1433)
    .WithDataVolume("cats-aspire-data")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpointProxySupport(false);
#pragma warning restore ASPIREPROXYENDPOINTS001
    
    
var catsDb = sql.AddDatabase("CatsDb");

var rabbit = builder.AddRabbitMQ("rabbit",
        userName: rabbitUser,
        password: rabbitPassword,
        port: 5672)
    .WithDataVolume("cats-aspire-rabbit")
    .WithManagementPlugin(port: 15672)
    .WithLifetime(ContainerLifetime.Persistent);

var cats = builder.AddProject<Projects.Server_UI>("cats", configure: project =>
    {
        // Exclude launchSettings on publish
        project.ExcludeLaunchProfile = publishing;
    })
    .WithReference(catsDb)
    .WithReference(rabbit)
    .WaitFor(sql);

if (publishing)
{
    cats.WithHttpEndpoint(port: 8080, targetPort: 8080)
        .WithReplicas(3);
}

builder.Build().Run();