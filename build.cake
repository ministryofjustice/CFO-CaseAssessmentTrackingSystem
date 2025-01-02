#addin nuget:?package=SharpZipLib&version=1.4.2
#addin nuget:?package=Cake.Compression&version=0.3.0

var target = Argument("target", "Test");
var configuration = Argument("configuration", "Release");
var fromVersion = Argument("fromVersion", "");
var migrationName = Argument("migrationName", "");

Task("Clean")
    .Description("Cleans all the bin and obj folders for the solution")
    .Does(() =>{
        CleanDirectories($"./src/**/bin/");
        CleanDirectories($"./src/**/obj/");
    });

Task("Build")
    .Description("Builds the solution in the given configuration. (Defaults to Release)")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetBuild("./cats.sln", new DotNetBuildSettings
    {
        Configuration = configuration,
    });
});

Task("Test")
    .Description("Runs all the unit tests in the solution")
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
    .Description("Publishes the Server.UI project, and compresses the output as ./publish/build-artifacts.zip")
    .IsDependentOn("Test")
    .Does(() => {
        CleanDirectory("./publish");

        var settings = new DotNetPublishSettings
            {
                Configuration = configuration,
                OutputDirectory = "./publish/workspace/"
            };

        DotNetPublish("./cats.sln", settings);
        ZipCompress("./publish/workspace", "./publish/build-artifacts.zip");
        CleanDirectory("./publish/workspace");
        DeleteDirectory("./publish/workspace", new DeleteDirectorySettings{
            Force = true,
            Recursive = true
        });
    });

Task("Script")
    .Description("Generates a migration script.")
    .IsDependentOn("Build")
    .Does(() =>{

        var migrationProject = "src/Infrastructure/Infrastructure.csproj";
        var startupProject = "src/Server.UI/Server.UI.csproj";
        var dbContext = $"Cfo.Cats.Infrastructure.Persistence.ApplicationDbContext";
        
        var result = StartProcess("dotnet", $"ef migrations script {fromVersion} --no-build --configuration {configuration} --project {migrationProject} --startup-project {startupProject} --context {dbContext} --idempotent -o ./publish/MigrationScript.sql");
        if(result != 0)
        {
            Error("Failed to generate migration script");
        }
    });

Task("AddMigration")
    .Description("Adds a new migration")
    .IsDependentOn("Build")
    .Does(() => {
        
        if(string.IsNullOrEmpty(migrationName))
        {
            throw new InvalidOperationException("You need to pass a migration name");
        }

        var migrationProject = "src/Infrastructure/Infrastructure.csproj";
        var startupProject = "src/Server.UI/Server.UI.csproj";
        var dbContext = $"Cfo.Cats.Infrastructure.Persistence.ApplicationDbContext";
        
        var result = StartProcess("dotnet", $"ef migrations add {migrationName} --no-build --configuration {configuration} --project {migrationProject} --startup-project {startupProject} --context {dbContext}");
        if(result != 0)
        {
            throw new InvalidOperationException("Failed to add migration");
        }

    });

RunTarget(target);
