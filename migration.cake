var migrationName = Argument("name", "NewMigration");
var infraProject  = "./src/Infrastructure/Infrastructure.csproj";
var startupProject = "./src/Server.UI/Server.UI.csproj";

Task("BuildServerUi")
    .Does(() =>
{
    DotNetBuild(startupProject, new DotNetBuildSettings {
        Configuration = "Debug",
        NoRestore = false
    });
});

Task("AddMigration")
    .IsDependentOn("BuildServerUi")
    .Does(() =>
{
    var settings = new ProcessSettings {
        Arguments = $"ef migrations add {migrationName} " +
                    $"--project {infraProject} " +
                    $"--startup-project {startupProject} " + 
                    $"--configuration Debug " + 
                    $"--output-dir Persistence/Migrations " +
                    "--no-build",
        WorkingDirectory = MakeAbsolute(Directory("./"))
    };

    StartProcess("dotnet", settings);
});

Task("Default")
    .IsDependentOn("AddMigration");

RunTarget("Default");