
var target = Argument("Target", "Default");
var migrationsProject = "./src/Infrastructure/Infrastructure.csproj";
var startupProject    = "./src/Server.UI/Server.UI.csproj";

Task("Clean")
    .Does(() => {
        CleanDirectory("./artifacts");
    });
    
Task("Restore")
    .Does(() => {
        DotNetRestore("./");
    });
    
Task("Build")
    .IsDependentOn("Restore")
    .Does(() => {
        DotNetBuild("./cats.sln", new DotNetBuildSettings(){
            Configuration = "Release",
            NoRestore = true
        });
    });
    
 Task("Test")
    .IsDependentOn("Build")
    .Does(() =>{
        DotNetTest("./", new DotNetTestSettings{
            NoBuild = true,
            NoRestore = true,
            Configuration = "Release"
        });
    });

Task("Publish UI")
    .IsDependentOn("Test")
    .Does(() =>
{
    DotNetPublish("./src/Server.UI/Server.UI.csproj",
        new DotNetPublishSettings
        {
            Configuration = "Release",
            OutputDirectory = "./artifacts/publish/Server.UI",
            NoRestore = true,
            NoBuild = true
        });

});
 
Task("Publish Migrations")
    .IsDependentOn("Publish UI")
    .Does(() =>
{
    DotNetPublish("./src/DatabaseMigrator/DatabaseMigrator.csproj",
        new DotNetPublishSettings
        {
            Configuration = "Release",
            OutputDirectory = "./artifacts/publish/DatabaseMigrator",
            NoRestore = true,
            NoBuild = true
        });

});

Task("Default")
    .IsDependentOn("Publish Migrations");
    
RunTarget(target);
