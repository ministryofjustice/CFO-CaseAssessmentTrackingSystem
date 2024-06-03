# Overview

HMPPS Creating Future Opportunities (CFO) utilise the Case Assessment and Tracking System (CATS) to support delivery of https://www.CreatingFutureOpportunities.gov.uk (CFO Evolution). The programme utilises external funding to perform rehabilitative services with offenders in custody and the community. Approx. 600 users from non-government organisations use CATS to record work performed with offenders creating an evidence base that supports performance management, payments to providers, ongoing research and audits from external bodies.

# Interfaces/Systems (Backend interface for surfacing the data to the front end) 

* Dotnet Core v8 - https://dotnet.microsoft.com/en-us/download
* Dotnet Core v5 - (for Identity Server 4.0)
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

To use Sqlite set the following in appsettings.json

```json
{
  "DatabaseSettings": {
    "DbProvider": "sqlite",
    "ConnectionString": "Data Source=./cats.db;"
  }
}
```

For MSSQL server you can use the following. If you are not running local db on windows, you will need to adjust the connection string.

```json
  "DatabaseSettings": {
    "DbProvider": "mssql",
    "ConnectionString": "Server=(LocalDB)\MSSQLLocalDB;Database=CatsDb;Integrated Security=True"
  },
```

# Status

[![.NET Core Unit Tests](https://github.com/ministryofjustice/CFO-CaseAssessmentTrackingSystem/actions/workflows/unittest.yml/badge.svg?branch=develop)](https://github.com/ministryofjustice/CFO-CaseAssessmentTrackingSystem/actions/workflows/unittest.yml)





