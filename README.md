[![.NET Core Unit Tests](https://github.com/ministryofjustice/CFO-CaseAssessmentTrackingSystem/actions/workflows/unittest.yml/badge.svg?branch=develop)](https://github.com/ministryofjustice/CFO-CaseAssessmentTrackingSystem/actions/workflows/unittest.yml)

# Overview

HMPPS Creating Future Opportunities (CFO) utilise the Case Assessment and Tracking System (CATS) to support delivery of https://www.CreatingFutureOpportunities.gov.uk (CFO Evolution). The programme utilises external funding to perform rehabilitative services with offenders in custody and the community. Approx. 600 users from non-government organisations use CATS to record work performed with offenders creating an evidence base that supports performance management, payments to providers, ongoing research and audits from external bodies.

# Interfaces/Systems (Backend interface for surfacing the data to the front end)

* .NET 9 - https://dotnet.microsoft.com/en-us/download
* ASP.NET Core

# Mechanism (How does it communicate with other systems? Frequency of data pull/push, reporting, events etc) 

Interactions with other systems are currently performed using a manual file transfer process between networks, however in the next 12 months it is planned that CATS will interact with ministryofjustice/cfo-dms2 

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

The project supports MSSQL.


## Cake scripts.

To publish the code and generate an indempotent sql update script.

```powershell
dotnet tool restore
dotnet cake --target Publish 
```

## Database Migrations

```powershell
dotnet cake --target Migrate 
```

### Update the database

*Note, the database will automatically update when the server ui project is ran in development environment*

```powershell
.\ModifyDatabase.ps1 -Action update
```

### Add a migration

*Running this will generate a new migration based on changes to the model*

```powershell
dotnet cake --target=AddMigration --MigrationName=Temp   
```

By default the migration will go to the ApplicationDbContext. You can modify this by passing through another argument.

```powershell
dotnet cake --target=AddMigration --MigrationName=Temp --context=ManagementInformationDbContext
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

# Publishing (preview)

This repository uses [Aspire]("https://github.com/dotnet/aspire") for service composition (dependency injection, service discovery, and configuration management). 

Aspire is also used to provide a Kubernetes publishing workflow that is currently in preview: container image build & push, and generation/packaging of Kubernetes manifests & Helm charts.

Prerequisites
- Docker (for image builds)
- Access to a container registry (credentials configured)
- kubectl and a valid kubeconfig with cluster access
- Helm (for chart-based deployments)

### Build and publish image
```
IMAGE_NAME=hmpps-cfo/cats
TAG=latest
REGISTRY=registry.mycorp.com:1234

# Locally
dotnet publish src/Server.UI/Server.UI.csproj \
    --configuration Release \
    --os linux \
    --arch x64 \
    --target:PublishContainer \
    --property:ContainerRepository=$IMAGE_NAME \
    --property:ContainerImageTag=$TAG

# or to a remote registry (with the ContainerRegistry property)
dotnet publish src/Server.UI/Server.UI.csproj \
    --configuration Release \
    --os linux \
    --arch x64 \
    --target:PublishContainer \
    --property:ContainerRegistry=$REGISTRY \
    --property:ContainerRepository=$IMAGE_NAME \
    --property:ContainerImageTag=$TAG
```

### Generate Kubernetes manifests & Helm charts
```
dotnet aspire publish -o aspire-output
```

### Deploy (using Helm upgrade)
```
helm upgrade --install aspire ./aspire-output --namespace default \
    --set parameters.cats.cats_image=$IMAGE
```