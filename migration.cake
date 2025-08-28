var migrationName = Argument("name", "NewMigration");
var infraProject  = "./src/Infrastructure/Infrastructure.csproj";
var startupProject = "./src/Server.UI/Server.UI.csproj";

Task("BuildInfra")
    .Does(() =>
{
    DotNetBuild(infraProject, new DotNetBuildSettings {
        Configuration = "Debug",
        NoRestore = false
    });
});

Task("AddMigration")
    .IsDependentOn("BuildInfra")
    .Does(() =>
{
    var settings = new ProcessSettings {
        Arguments = $"ef migrations add {migrationName} " +
                    $"--project {infraProject} " +
                    $"--startup-project {startupProject}",
        WorkingDirectory = MakeAbsolute(Directory("./"))
    };

    StartProcess("dotnet", settings);
});

Task("Default")
    .IsDependentOn("AddMigration");

RunTarget("Default");