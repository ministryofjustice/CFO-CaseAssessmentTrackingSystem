
# step one build the project

#remove the publish fodler

$sourcePath = ".\publish\*"
$destinationPath = ".\publish\build-artifacts.zip"
$publishPath = ".\publish"

if(Test-Path $publishPath) {
    Write-Host "Cleaning publish path"
    Remove-Item -Recurse $publishPath
}

Write-Host "Restoring nuget packages" -ForegroundColor Green
dotnet restore 

Write-Host "Restoring tools" -ForegroundColor Green
dotnet tool restore

Write-Host "Building" -ForegroundColor Green
dotnet build --configuration Release --no-restore

Write-Host "Running unit tests" -ForegroundColor Green
dotnet test --configuration Release --no-build --verbosity normal

Write-Host "Publishing package" -ForegroundColor Green
dotnet publish --no-build --configuration Release --output $publishPath

Write-Host "Generating indempotent migration script" -ForegroundColor Green
dotnet ef migrations script --no-build --configuration Release --project src/Migrators/Migrators.MSSQL/Migrators.MSSQL.csproj --startup-project src/Server.UI/Server.UI.csproj --context Cfo.Cats.Infrastructure.Persistence.ApplicationDbContext --idempotent -o ./publish/Migration.sql

Write-Host "Compressing artifacts" -ForegroundColor Green
Compress-Archive -Path $sourcePath -DestinationPath $destinationPath

Write-Host "Cleaning publish folder" -ForegroundColor Green

Get-ChildItem -Path $publishPath -Exclude build-artifacts.zip,Migration.sql | ForEach-Object {
    if ($_.FullName -ne $destinationPath) {
        Remove-Item -Recurse -Force $_.FullName
    }
}