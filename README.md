# CFO Case Assessment Tracking System (CATS)
[![Ministry of Justice Repository Compliance Badge](https://github-community.service.justice.gov.uk/repository-standards/api/CFO-CaseAssessmentTrackingSystem/badge?style=flat)](https://github-community.service.justice.gov.uk/repository-standards/CFO-CaseAssessmentTrackingSystem)
[![Docker Repository on ghcr](https://img.shields.io/badge/ghcr.io-repository-2496ED.svg?logo=docker)](https://ghcr.io/ministryofjustice/hmpps-cfo-cats)
[![Pipeline](https://github.com/ministryofjustice/CFO-CaseAssessmentTrackingSystem/actions/workflows/pipeline.yml/badge.svg?branch=main)](https://github.com/ministryofjustice/CFO-CaseAssessmentTrackingSystem/actions/workflows/pipeline.yml)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)

## Contents

- [About this project](#about-this-project)
  - [External dependencies](#external-dependencies)
  - [Infrastructure](#infrastructure)

- [Get started](#get-started)
  - [Prerequisites](#prerequisites)

- [Usage](#usage)
  - [Running the application locally](#running-the-application-locally)
  - [Build automation with Cake](#build-automation-with-cake)

## About this project

HMPPS Creating Future Opportunities (CFO) uses the Case Assessment and Tracking System (CATS) to support delivery of [CFO Evolution](https://www.CreatingFutureOpportunities.gov.uk), a programme funded externally to deliver rehabilitative services with offenders in custody and the community. Approx. 600 users (100 concurrent) from non-government organisations use CATS to record work performed with offenders, creating an evidence base that supports performance management, payments to providers, ongoing research and audits from external bodies.

It is built using [ASP.NET](https://dotnet.microsoft.com/en-us/apps/aspnet) (for cross-platform development), and relies on the following technologies:
* C#
* Blazor Server & [MudBlazor](https://mudblazor.com/) (UI framework & components)
* Entity Framework Core & Microsoft SQL Server, with [SQL Project](https://learn.microsoft.com/en-us/sql/tools/sql-database-projects/sql-database-projects) for schema management
* [Quartz](https://www.quartz-scheduler.net/) (background jobs)
* [Rebus](https://github.com/rebus-org/Rebus) with RabbitMQ (messaging/outbox)
* [.NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/) (local orchestration)

### External dependencies
This solution is dependent on:
- [CFO Data Management System (DMS)](https://github.com/ministryofjustice/CFO-DataManagementSystem) for information sourced from the National Offender Management Information System (NOMIS) via Offloc, and nDelius via the cfoextract. DMS aggregates this information and exposes it to CATS via the DMS API.

### Infrastructure

This service is deployed to the [MoJ Cloud Platform](https://user-guide.cloud-platform.service.justice.gov.uk) using the shared [generic-service Helm chart](https://github.com/ministryofjustice/hmpps-helm-charts/tree/main/charts/generic-service), see [`helm_deploy`](helm_deploy/) for configuration.

## Get started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker](https://docs.docker.com/engine/install/)
- **Visual Studio Code users**:
    - [C# Dev Kit Extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
    - [Aspire Extension (& CLI)](https://marketplace.visualstudio.com/items?itemName=microsoft-aspire.aspire-vscode)

## Usage

### Running the application locally
The recommended way to run and debug the application is using .NET Aspire.
- **Using Visual Studio Code**: open the project and press `F5`, selecting the *Default Configuration*.
- **Using Visual Studio or other IDEs**: From the debug configuration dropdown, select `Cats.AppHost` and start the application.

On startup, Aspire automatically applies the [CatsDb SQL Project](src/Database/CatsDb/) schema to the local SQL Server container before starting the application. The [Database Seeding](src/DatabaseSeeding/) project seeds any required reference data.

### Build automation with Cake

This repository includes a file-based Cake script at `cake.cs`. Run it from the repository root with:

```bash
dotnet cake.cs
```

By default this runs the `Publish` target with the `Release` configuration. The full target chain is:

`Clean -> Restore -> Build -> Test -> Publish`

You can run a specific target or change the configuration by passing arguments to the script:

```bash
# Build without publishing
dotnet cake.cs --target=Build

# Run tests
dotnet cake.cs --target=Test

# Publish using Debug configuration
dotnet cake.cs --target=Publish --configuration=Debug
```

Available targets:

- `Clean`
- `Restore`
- `Build`
- `Test`
- `Publish`

The `Publish` target writes output to:

- `artifacts/Server.UI`
- `artifacts/DatabaseSeeding`
- `artifacts/CatsDb.dacpac`