using System.Data.Common;
using Microsoft.SqlServer.Dac;

// Deploys the CatsDb schema by publishing the compiled DACPAC with DacFx — the same
// engine sqlpackage wraps. Runs from the shared "cfo-cats" image as a pre-install/
// pre-upgrade Helm hook Job (see helm_deploy/cats/templates/migrator-hook.yaml), so
// the schema is migrated ahead of the new application code on every release.

const string connectionStringEnv = "ConnectionStrings__CatsDb";

var connectionString = Environment.GetEnvironmentVariable(connectionStringEnv);
if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.Error.WriteLine($"ERROR: environment variable '{connectionStringEnv}' is not set.");
    return 1;
}

var databaseName = GetTargetDatabaseName(connectionString);
if (string.IsNullOrWhiteSpace(databaseName))
{
    Console.Error.WriteLine("ERROR: the connection string must specify a target database (Database / Initial Catalog).");
    return 1;
}

var dacpacPath = Path.Combine(AppContext.BaseDirectory, "CatsDb.dacpac");
if (!File.Exists(dacpacPath))
{
    Console.Error.WriteLine($"ERROR: DACPAC not found at '{dacpacPath}'.");
    return 1;
}

try
{
    using var package = DacPackage.Load(dacpacPath);

    var services = new DacServices(connectionString);
    services.Message += (_, e) => Console.WriteLine(e.Message);
    services.ProgressChanged += (_, e) => Console.WriteLine($"[{e.Status}] {e.Message}");

    var options = new DacDeployOptions
    {
        // Preserve the previous sqlpackage behaviour (/p:BlockOnPossibleDataLoss=false).
        BlockOnPossibleDataLoss = false,
    };

    Console.WriteLine($"Deploying schema to database '{databaseName}' from '{Path.GetFileName(dacpacPath)}'...");
    services.Deploy(package, databaseName, upgradeExisting: true, options);
    Console.WriteLine("Schema deployed successfully.");
    return 0;
}
catch (DacServicesException ex)
{
    Console.Error.WriteLine($"ERROR: schema deployment failed: {ex.Message}");
    if (ex.InnerException is not null)
    {
        Console.Error.WriteLine(ex.InnerException.ToString());
    }

    return 1;
}

// Extract the target database name without taking a direct Microsoft.Data.SqlClient
// dependency — DacFx brings its own version-matched SqlClient/MSAL closure, and forcing
// a different SqlClient version leaves it unable to load the MSAL assembly it expects.
static string? GetTargetDatabaseName(string connectionString)
{
    var builder = new DbConnectionStringBuilder { ConnectionString = connectionString };

    foreach (var key in new[] { "Initial Catalog", "Database" })
    {
        if (builder.TryGetValue(key, out var value))
        {
            var name = value?.ToString();
            if (!string.IsNullOrWhiteSpace(name))
            {
                return name;
            }
        }
    }

    return null;
}
