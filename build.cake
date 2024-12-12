#addin nuget:?package=SharpZipLib&version=1.4.2
#addin nuget:?package=Cake.Compression&version=0.3.0

var target = Argument("target", "Test");
var configuration = Argument("configuration", "Release");
var fromVersion = Argument("fromVersion", "");
var context = Argument("context", "ApplicationDbContext");

Task("Clean")
    .WithCriteria(c => c.@HasArgument("rebuild"))
    .Does(() =>{
        CleanDirectories($"./src/**/bin/{configuration}");
    });

Task("Build")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetBuild("./cats.sln", new DotNetBuildSettings
    {
        Configuration = configuration,
    });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetTest("./cats.sln", new DotNetTestSettings
    {
        Configuration = configuration,
        NoBuild = true,
    });
});

Task("Publish")
    .IsDependentOn("Test")
    .Does(() => {
        CleanDirectory("./publish");

        var settings = new DotNetPublishSettings
            {
                Configuration = configuration,
                OutputDirectory = "./publish/workspace/"
            };

        DotNetPublish("./cats.sln", settings);

        var migrationProject = "src/Migrators/Migrators.MSSQL/Migrators.MSSQL.csproj";
        var startupProject = "src/Server.UI/Server.UI.csproj";
        var context = "Cfo.Cats.Infrastructure.Persistence.ApplicationDbContext";
        var result = StartProcess("dotnet", $"ef migrations script --no-build --configuration {configuration} --project {migrationProject} --startup-project {startupProject} --context {context} --idempotent -o ./publish/MigrationScript.sql");

        if(result != 0)
        {
            Error("Failed to generate migration script");
        }

        ZipCompress("./publish/workspace", "./publish/build-artifacts.zip");
        CleanDirectory("./publish/workspace");
        DeleteDirectory("./publish/workspace", new DeleteDirectorySettings{
            Force = true,
            Recursive = true
        });
    });

Task("Migrate")
    .IsDependentOn("Test")
    .Does(() =>{
        var migrationProject = "src/Migrators/Migrators.MSSQL/Migrators.MSSQL.csproj";
        var startupProject = "src/Server.UI/Server.UI.csproj";
        var dbContext = $"Cfo.Cats.Infrastructure.Persistence.{context}";

        
        var result = StartProcess("dotnet", $"ef migrations script {fromVersion} --no-build --configuration {configuration} --project {migrationProject} --startup-project {startupProject} --context {dbContext} --idempotent -o ./publish/{context}-MigrationScript.sql");
        if(result != 0)
        {
            Error("Failed to generate migration script");
        }
    });

RunTarget(target);
