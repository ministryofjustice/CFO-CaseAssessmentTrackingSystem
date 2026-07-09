#:package Microsoft.SqlServer.DacFx
#:property PublishAot=false

using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Dac;

// Deploys the CatsDb schema from the compiled DACPAC via DacFx (the engine sqlpackage wraps).
// A .NET 10 file-based app; runs as a pre-install/pre-upgrade Helm hook Job from the shared
// hmpps-cfo-cats image (helm_deploy/cats/templates/migrator-hook.yaml). Run locally with:
//   dotnet build src/Database/CatsDb/CatsDb.sqlproj -c Release
//   ConnectionStrings__CatsDb="..." DACPAC_PATH=src/Database/CatsDb/bin/Release/CatsDb.dacpac \
//     dotnet run scripts/migrate-database.cs

const string connectionStringEnv = "ConnectionStrings__CatsDb";

var connectionString = Environment.GetEnvironmentVariable(connectionStringEnv);
if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.Error.WriteLine($"ERROR: environment variable '{connectionStringEnv}' is not set.");
    return 1;
}

// SqlConnectionStringBuilder via DacFx's transitive Microsoft.Data.SqlClient — no direct
// PackageReference (that would force the central 7.0.2 pin and break DacFx's MSAL closure).
var databaseName = new SqlConnectionStringBuilder(connectionString).InitialCatalog;
if (string.IsNullOrWhiteSpace(databaseName))
{
    Console.Error.WriteLine("ERROR: the connection string must specify a target database (Database / Initial Catalog).");
    return 1;
}

// The image ships CatsDb.dacpac next to the app; DACPAC_PATH overrides it for local runs.
var dacpacPath = Environment.GetEnvironmentVariable("DACPAC_PATH");
if (string.IsNullOrWhiteSpace(dacpacPath))
{
    dacpacPath = Path.Combine(AppContext.BaseDirectory, "CatsDb.dacpac");
}

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
