
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
    
 Task("Publish")
     .IsDependentOn("Test")
     .Does(() =>
 {
     DotNetPublish("./src/Server.UI/Server.UI.csproj",
         new DotNetPublishSettings {
             Configuration = "Release",
             OutputDirectory = "./artifacts/publish",
             NoRestore = true,
             NoBuild = true
         });
 });
 
 Task("ScriptMigration")
     .IsDependentOn("Publish")
     .Does(() =>
 {
     var settings = new ProcessSettings {
             Arguments = "ef migrations script --no-build " +
                         "--project ./src/Infrastructure/Infrastructure.csproj " +
                         "--startup-project ./src/Server.UI/Server.UI.csproj " +
                         "--configuration Release " +
                         "--output ./artifacts/migrations.sql --idempotent",
             WorkingDirectory = Context.Environment.WorkingDirectory // force repo root
         };
         
     StartProcess("dotnet", settings);
 });
 
 Task("Default")
    .IsDependentOn("ScriptMigration");
    
 RunTarget(target);
