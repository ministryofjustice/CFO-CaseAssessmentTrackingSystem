# Overview

HMPPS Creating Future Opportunities (CFO) utilise the Case Assessment and Tracking System (CATS) to support delivery of https://www.CreatingFutureOpportunities.gov.uk (CFO Evolution). The programme utilises external funding to perform rehabilitative services with offenders in custody and the community. Approx. 600 users from non-government organisations use CATS to record work performed with offenders creating an evidence base that supports performance management, payments to providers, ongoing research and audits from external bodies.

# Interfaces/Systems (Backend interface for surfacing the data to the front end) 

* Dotnet Core v8 - https://dotnet.microsoft.com/en-us/download
* ASP.NET Core

# Mechanism (How does it communicate with other systems? Frequency of data pull/push, reporting, events etc) 

Interactions with other systems are currently performed using a manual file transfer process between networks, however in the next 12 months it is planned that CATS+ will interact with ministryofjustice/cfo-dms2 

# Technology (What's the technology that drives the product? i.e. Azure, java script etc) 

* ASP.NET Core (Blazor)
* C#
* LINQ
* Entity Framework
* SQL Server
* HTML, CSS, Javascript

# No. of users 

600 (approx. 100 concurrent)

# Development Environment

This has been developed on Windows 11 using Visual Studio 2022, Visual Studio Code and JetBrains Rider

# Queries

Any queries, please contact andrew.grocott@justice.gov.uk

v1.0-alpha

# Building and Tool Restore

The project supports MSSQL and (for development and demo sites) Sqlite and in-memory.

By convention the appsettings.Development json is configured to use sqlite.

To use an in memory database (only good for development) alter the appsettings for use In-Memory database to true. This will
override any other database settings.

```json
{
  "UseInMemoryDatabase": true,
}
```


For MSSQL server you can use the following. If you are not running local db on windows, you will need to adjust the connection string.

```json
  "DatabaseSettings": {
    "DbProvider": "mssql",
    "ConnectionString": "Server=(LocalDB)\MSSQLLocalDB;Database=CatsDb;Integrated Security=True"
  },
```

## Database Migrations

There is a powershell script in the root of the folder called ModifyDatabase.ps1. This can be used to 

### Update the database

*Note, the database will automatically update when the server ui project is ran in development environment*

```powershell
.\ModifyDatabase.ps1 -Action update
```

### Add a migration

*Running this will generate a new migration based on changes to the model*

```powershell
.\ModifyDatabase.ps1 -Action add -MigrationName NewMigrationName
```

### Remove a migration

*This will remove the last migration from the code. Useful if you need to make changes due to a malformed migration or a noticed error. Should not be applied to migrations that have been ran against a production envrionment*

```powershell
.\ModifyDatabase.ps1 -Action remove
```

### Drop the database

*This will force a database to be dropped. Useful when developing and you need to re-seed any data etc.*

```powershell
 .\ModifyDatabase.ps1 -Action drop
```

### Seed the database

*This will run the passed through SQL file against the database*

```powershell
.\ModifyDatabase.ps1 -Action seed -SqlFilePath .\db\seed\001_development_seed.sql
```

### Example usage

Chaining the commands together it is very easy to refresh your local database by running the following

```powershell

.\ModifyDatabase.ps1 -Action drop | `
.\ModifyDatabase.ps1 -Action update | `
.\ModifyDatabase.ps1 -Action seed -SqlFilePath .\db\seed\001_development_seed.sql

```




