using AppHost;
using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddForwardedHeaders();

var rabbitMq = builder.AddRabbitMQ("eventbus")
                .WithManagementPlugin();

var sqlPassword = builder.AddParameter("SqlPassword", true);

var mssql = builder.AddSqlServer("mssql", sqlPassword, port: 1433);

var identityDb = mssql.AddDatabase("IdentityDb");
var enrolmentsDb = mssql.AddDatabase("EnrolmentsDb");
var administrationDb = mssql.AddDatabase("AdministrationDb");
var documentsDb = mssql.AddDatabase("DocumentsDb");

var identity = builder.AddProject<Projects.Identity_Api>("identity-api")
    .WithReference(identityDb)
    //HACK: net core 5 uses a different environment name 
    .WithEnvironment("ENVIRONMENT", "DEVELOPMENT");
    
var idpHttp = identity.GetEndpoint("http");

var enrolmentsApi = builder.AddProject<Projects.Enrolments_Api>("enrolments-api")
    .WithReference(rabbitMq)
    .WithReference(enrolmentsDb)
    .WithEnvironment("Identity__Url", idpHttp);

var administrationApi = builder.AddProject<Projects.Administration_Api>("administration-api")
    .WithReference(rabbitMq)
    .WithReference(administrationDb)
    .WithEnvironment("Identity__Url", idpHttp);

var documentsApi = builder.AddProject<Projects.Documents_Api>("documents-api")
    .WithReference(rabbitMq)
    .WithReference(documentsDb)
    .WithEnvironment("Identity__Url", idpHttp);

var webApp = builder.AddProject<Projects.WebApp>("webapp", launchProfileName: "https")
    .WithReference(enrolmentsApi)
    .WithReference(administrationApi)
    .WithReference(documentsApi)
    .WithReference(rabbitMq)
    .WithEnvironment("IdentityUrl", idpHttp);

webApp.WithEnvironment("CallBackUrl", webApp.GetEndpoint("https"));

identity
    .WithEnvironment("EnrolmentApiClient", enrolmentsApi.GetEndpoint("http"))
    .WithEnvironment("AdministrationApiClient", administrationApi.GetEndpoint("http"))
    .WithEnvironment("DocumentApiClient", documentsApi.GetEndpoint("http"))
    .WithEnvironment("WebAppClient", webApp.GetEndpoint("https"));

//builder.AddProject<Projects.Aspire_Dashboard>(KnownResourceNames.AspireDashboard);

builder.Build().Run();
