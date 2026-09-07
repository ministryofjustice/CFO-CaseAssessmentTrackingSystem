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
        
        var dirsToClean = GetDirectories("./**/bin");
        dirsToClean.Add(GetDirectories("./**/obj"));
        dirsToClean.Add("./artifacts");

        foreach(var dir in dirsToClean)
        {
            Information($"Cleaning directory {dir}");
            CleanDirectory(dir);    
        }

        var filesToClean = GetFiles("./*.zip");

        foreach(var file in filesToClean)
        {
            Information($"Deleting file {file}");
            DeleteFile(file);
        }

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
        // Mirror the Dockerfile's combined image: each app is published into its own
        // directory (ui / worker / consumers / seeder / migrator) so their appsettings.json
        // files do not collide. Keep these output names in sync with the Dockerfile.

        DotNetPublish("./src/Server.UI/Server.UI.csproj", new DotNetPublishSettings()
        {
           Configuration = configuration,
           NoBuild = true,
           NoRestore = true,
           OutputDirectory = "./artifacts/ui",
        });

        DotNetPublish("./src/Worker/Worker.csproj", new DotNetPublishSettings()
        {
           Configuration = configuration,
           NoBuild = true,
           NoRestore = true,
           OutputDirectory = "./artifacts/worker",
        });

        DotNetPublish("./src/Cats.Consumers/Cats.Consumers.csproj", new DotNetPublishSettings()
        {
           Configuration = configuration,
           NoBuild = true,
           NoRestore = true,
           OutputDirectory = "./artifacts/consumers",
        });

        DotNetPublish("./src/DatabaseSeeding/DatabaseSeeding.csproj", new DotNetPublishSettings()
        {
           Configuration = configuration,
           NoBuild = true,
           NoRestore = true,
           OutputDirectory = "./artifacts/seeder",
        });

        DotNetPublish("./src/Database/CatsDb/CatsDb.sqlproj", new DotNetPublishSettings()
        {
            Configuration = configuration,
            OutputDirectory = "./artifacts/migrator",
        });

        DotNetPublish("./scripts/migrate-database.cs", new DotNetPublishSettings()
        {
            Configuration = configuration,
            OutputDirectory = "./artifacts/migrator",
        });

    });

RunTarget(target); 