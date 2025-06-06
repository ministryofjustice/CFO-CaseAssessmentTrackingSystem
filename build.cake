#addin nuget:?package=SharpZipLib&version=1.4.2
#addin nuget:?package=Cake.Compression&version=0.3.0

var target = Argument("target", "Test");
var configuration = Argument("configuration", "Release");
var fromVersion = Argument("fromVersion", "");
var migrationName = Argument("migrationName", "");

Task("Clean")
    .Description("Cleans all the bin and obj folders for the solution")
    .Does(() => {
        Information("Cleaning the solution");
        try 
        {
            CleanDirectories($"./src/**/bin/");
            CleanDirectories($"./src/**/obj/");
            Information("Clean completed successfully");
        }
        catch (Exception ex)
        {
            Error($"Clean failed: {ex.Message}");
            throw;
        }
    });

Task("Build")
    .Description("Builds the solution in the given configuration. (Defaults to Release)")
    .IsDependentOn("Clean")
    .Does(() => {
        Information("Building the solution");
        try 
        {
            DotNetBuild("./cats.sln", new DotNetBuildSettings
            {
                Configuration = configuration,
                NoRestore = false
            });
            Information("Build completed successfully");
        }
        catch (Exception ex)
        {
            Error($"Build failed: {ex.Message}");
            throw;
        }
    });

Task("Test")
    .Description("Runs all the unit tests in the solution")
    .IsDependentOn("Build")
    .Does(() => {
        Information("Running all unit tests");
        try 
        {
            DotNetTest("./cats.sln", new DotNetTestSettings
            {
                Configuration = configuration,
                NoBuild = true,
                Loggers = new[] { "trx" },
                ResultsDirectory = "./TestResults/"
            });
            Information("Tests completed successfully");
        }
        catch (Exception ex)
        {
            Error($"Tests failed: {ex.Message}");
            throw;
        }
    });

Task("Publish")
    .Description("Publishes the Server.UI project")
    .IsDependentOn("Test")
    .Does(() => {
        Information("Publishing the Server.UI project");
        try 
        {
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
            Information("Publishing completed successfully");
        }
        catch (Exception ex)
        {
            Error($"Publishing failed: {ex.Message}");
            throw;
        }
    });

Task("Script")
    .Description("Generates a migration script with clean build")
    .Does(() => {
        Information("Generating migration script");
        try 
        {
            var migrationProject = "src/Infrastructure/Infrastructure.csproj";
            var startupProject = "src/Server.UI/Server.UI.csproj";
            var dbContext = "Cfo.Cats.Infrastructure.Persistence.ApplicationDbContext";
            
            var result = StartProcess("dotnet", $"ef migrations script {fromVersion} --no-build --configuration {configuration} --project {migrationProject} --startup-project {startupProject} --context {dbContext} --idempotent -o ./publish/MigrationScript.sql");
            
            if (result != 0)
            {
                Error("Failed to generate migration script");
                throw new InvalidOperationException("Failed to generate migration script");
            }
            
            Information("Migration script generated successfully");
        }
        catch (Exception ex)
        {
            Error($"Script generation failed: {ex.Message}");
            throw;
        }
    });

Task("AddMigration")
    .Description("Adds a new migration")
    .IsDependentOn("Build")
    .Does(() => {
        Information("Adding new migration");
        try 
        {
            if (string.IsNullOrEmpty(migrationName))
            {
                Error("Migration name is required. Use --migrationName=YourMigrationName");
                throw new InvalidOperationException("Migration name is required");
            }

            var migrationProject = "src/Infrastructure/Infrastructure.csproj";
            var startupProject = "src/Server.UI/Server.UI.csproj";
            var dbContext = "Cfo.Cats.Infrastructure.Persistence.ApplicationDbContext";
            
            var result = StartProcess("dotnet", $"ef migrations add {migrationName} --no-build --configuration {configuration} --project {migrationProject} --startup-project {startupProject} --context {dbContext}");
            
            if (result != 0)
            {
                Error("Failed to add migration");
                throw new InvalidOperationException("Failed to add migration");
            }
            
            Information("Migration added successfully");
        }
        catch (Exception ex)
        {
            Error($"Adding migration failed: {ex.Message}");
            throw;
        }
    });

RunTarget(target);