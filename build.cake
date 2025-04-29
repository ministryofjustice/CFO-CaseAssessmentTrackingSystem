#addin nuget:?package=SharpZipLib&version=1.4.2
#addin nuget:?package=Cake.Compression&version=0.3.0

#r "Spectre.Console"

using Spectre.Console

var target = Argument("target", "Test");
var configuration = Argument("configuration", "Release");
var fromVersion = Argument("fromVersion", "");
var migrationName = Argument("migrationName", "");


Task("Clean")
    .Description("Cleans all the bin and obj folders for the solution")
    .Does(() =>{
        LogInformation("Cleaning the solution");
        CleanDirectories($"./src/**/bin/");
        CleanDirectories($"./src/**/obj/");
    });

Task("Build")
    .Description("Builds the solution in the given configuration. (Defaults to Release)")
    .IsDependentOn("Clean")
    .Does(() =>
{
    LogInformation("Building the solution");
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
    LogInformation("Running all unit tests");
    DotNetTest("./cats.sln", new DotNetTestSettings
    {
        Configuration = configuration,
        NoBuild = true,
    });
});

Task("Publish")
    .Description("Publishes the Server.UI project")
    .IsDependentOn("Test")
    .Does(() => {

        LogInformation("Publishing the Server.UI project");

        CleanDirectory("./publish");

        var settings = new DotNetPublishSettings
            {
                Configuration = configuration,
                OutputDirectory = "./publish/",
                NoBuild = true,
	        MSBuildSettings = new DotNetMSBuildSettings()
                .WithProperty("DebugType", "None")
                .WithProperty("DebugSymbols", "false")
		
            };

        DotNetPublish("./src/Server.UI/Server.UI.csproj", settings);
       
        LogInformation("Publishing complete");
    });

Task("Script")
    .Description("Generates a migration script.")
    .Does(() =>{

        bool rebuild = GetYesNoResponse("Would you like to perform a clean build?");

        if(rebuild)
        {
            LogInformation("Generating a script with a clean build");
            RunTarget("Build");
        }
        else
        {
            LogWarning("Generating a script without rebuild. This may result in an outdated build being used");
        }

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
    .Does(() => {


        bool rebuild = GetYesNoResponse("Would you like to perform a clean build?");
        
        if(rebuild)
        {
            LogInformation("Generating a migration with a clean build");
            RunTarget("Build");
        }
        else
        {
            LogWarning("Generating a migration without rebuild. This may result in an outdated build being used");
        }
        
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


void LogWarning(string message)
{
    AnsiConsole.MarkupLine($"[bold red1]:face_with_monocle: {message}[/]");
}

void LogInformation(string message)
{
    AnsiConsole.MarkupLine($"[bold green1]:thinking_face: {message}[/]");
}

bool GetYesNoResponse(string question, bool defaultAnswer = true)
{
    string defaultText = defaultAnswer ? "[Y/n]" : "[y/N]";
    
    while (true)
    {
        Console.Write($"{question} {defaultText}: ");
        string input = Console.ReadLine()?.Trim().ToLower();
        
        // Accept default on empty input
        if (string.IsNullOrEmpty(input))
            return defaultAnswer;
            
        if (input == "y" || input == "yes")
            return true;
        if (input == "n" || input == "no")
            return false;
            
        // Invalid input - prompt again
        Console.WriteLine("Please answer with 'y' or 'n'.");
    }
}

RunTarget(target);
