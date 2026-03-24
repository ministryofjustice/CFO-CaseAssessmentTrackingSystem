#:sdk Cake.Sdk@6.1.1

var target = Argument("target", "Publish");
var configuration = Argument("configuration", "Release");

var sln = "./cats.slnx";

Task("Clean")
    .Does(() =>
    {
        CleanDirectorySettings settings = new()
        {
            Force = true
        };
        CleanDirectories("./src/**/bin/{configuration}", settings);
        CleanDirectory("./artifacts", settings);
    });

Task("Restore")
    .Does(() => DotNetRestore(sln));

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() => DotNetBuild(sln, new DotNetBuildSettings
    {
        Configuration = configuration,
        NoRestore = true,
    }));

Task("Test")
    .IsDependentOn("Build")
    .Does(() => DotNetTest(sln, new DotNetTestSettings
    {
        Configuration = configuration,
        NoBuild = true,
        NoRestore = true,
    }));

Task("Publish")
    .IsDependentOn("Test")
    .Does(() =>
    {
        // publish the Server.UI
        DotNetPublish("./src/Server.UI/Server.UI.csproj", new DotNetPublishSettings()
        {
           Configuration = configuration,
           NoBuild = true,
           NoRestore = true,
           OutputDirectory = "./artifacts/Server.UI",
        });

        //publish the Database Migrator
        DotNetPublish("./src/DatabaseMigrator/DatabaseMigrator.csproj", new DotNetPublishSettings()
        {
           Configuration = configuration,
           NoBuild = true,
           NoRestore = true,
           OutputDirectory = "./artifacts/DatabaseMigrator",
        });
    });

RunTarget(target); 