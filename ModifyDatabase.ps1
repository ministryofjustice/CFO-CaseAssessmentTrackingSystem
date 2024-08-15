param (
    [string]$MigrationName,
    [ValidateSet('add', 'remove', 'drop', 'seed', 'update')]
    [string]$Action,
    [string]$SqlFilePath
)

function Add-Migration {
    param (
        [string]$Name
    )
    if (-not $Name) {
        Write-Host "Please provide a name for the migration."
        return
    }
    $command = "dotnet ef migrations add $Name --project src\Migrators\Migrators.MSSQL\Migrators.MSSQL.csproj --startup-project src\Server.UI\Server.UI.csproj --context Cfo.Cats.Infrastructure.Persistence.ApplicationDbContext --configuration Debug"
    Write-Host "Executing: $command"
    Invoke-Expression $command
}

function Remove-LastMigration {
    $command = "dotnet ef migrations remove --project src\Migrators\Migrators.MSSQL\Migrators.MSSQL.csproj --startup-project src\Server.UI\Server.UI.csproj --no-build --context Cfo.Cats.Infrastructure.Persistence.ApplicationDbContext --configuration Debug"
    Write-Host "Executing: $command"
    Invoke-Expression $command
}

function Remove-Database {
    $command = "dotnet ef database drop --project src\Migrators\Migrators.MSSQL\Migrators.MSSQL.csproj --startup-project src\Server.UI\Server.UI.csproj --no-build --context Cfo.Cats.Infrastructure.Persistence.ApplicationDbContext --configuration Debug --force"
    Write-Host "Executing: $command"
    Invoke-Expression $command
}

function Update-Database {
    $command = "dotnet ef database update --project src\Migrators\Migrators.MSSQL\Migrators.MSSQL.csproj --startup-project src\Server.UI\Server.UI.csproj --no-build --context Cfo.Cats.Infrastructure.Persistence.ApplicationDbContext --configuration Debug"
    Write-Host "Executing: $command"
    Invoke-Expression $command
}

function Invoke-SqlFile {
    param (
        [string]$FilePath
    )

    Write-Host "Received SQL file path: '$FilePath'"

    if (-not $FilePath) {
        Write-Host "SQL file path cannot be null or empty."
        return
    }

    try {
        $absoluteFilePath = Resolve-Path $FilePath -ErrorAction Stop | Select-Object -First 1 -ExpandProperty Path
        Write-Host "Resolved SQL file path: '$absoluteFilePath'"
    } catch {
        Write-Host "Error resolving SQL file path: $_"
        return
    }

    if (-not (Test-Path $absoluteFilePath)) {
        Write-Host "SQL file does not exist: $absoluteFilePath"
        return
    }

    $appSettingsPath = "src\Server.UI\appsettings.json"
    if (-not (Test-Path $appSettingsPath)) {
        Write-Host "appsettings.json not found at: $appSettingsPath"
        return
    }

    $appSettings = Get-Content -Raw -Path $appSettingsPath | ConvertFrom-Json
    $connectionString = $appSettings.DatabaseSettings.ConnectionString

    if (-not $connectionString) {
        Write-Host "Connection string not found in appsettings.json"
        return
    }

    try {
        $sqlScript = Get-Content -Raw -Path $absoluteFilePath

        $connection = New-Object System.Data.SqlClient.SqlConnection
        $connection.ConnectionString = $connectionString
        $connection.Open()

        $command = $connection.CreateCommand()
        $command.CommandText = $sqlScript
        $command.ExecuteNonQuery()

        Write-Host "SQL script executed successfully."

        $connection.Close()
    } catch {
        Write-Host "An error occurred: $_"
    }
}

switch ($Action) {
    'add' {
        Add-Migration -Name $MigrationName
    }
    'remove' {
        Remove-LastMigration
    }
    'drop' {
        Remove-Database
    }
    'update' {
        Update-Database
    }
    'seed' {
        Invoke-SqlFile -FilePath $SqlFilePath
    }
    default {
        Write-Host "Invalid action. Please specify 'add', 'remove', 'drop', 'update', or 'seed'."
    }
}
