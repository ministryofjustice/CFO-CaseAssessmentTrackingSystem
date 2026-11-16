# CFO Case Assessment Tracking System (CATS)
[![Ministry of Justice Repository Compliance Badge](https://github-community.service.justice.gov.uk/repository-standards/api/CFO-CaseAssessmentTrackingSystem/badge?style=flat)](https://github-community.service.justice.gov.uk/repository-standards/CFO-CaseAssessmentTrackingSystem)
[![Docker Repository on ghcr](https://img.shields.io/badge/ghcr.io-repository-2496ED.svg?logo=docker)](https://ghcr.io/ministryofjustice/hmpps-cfo-cats)
[![Pipeline](https://github.com/ministryofjustice/CFO-CaseAssessmentTrackingSystem/actions/workflows/pipeline.yml/badge.svg?branch=main)](https://github.com/ministryofjustice/CFO-CaseAssessmentTrackingSystem/actions/workflows/pipeline.yml)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)

HMPPS Creating Future Opportunities (CFO) uses CATS to support delivery of [CFO Evolution](https://www.CreatingFutureOpportunities.gov.uk), a programme providing rehabilitative services in custody and the community.

# Project overview

HMPPS Creating Future Opportunities (CFO) uses the Case Assessment and Tracking System (CATS) to support delivery of [CFO Evolution](https://www.CreatingFutureOpportunities.gov.uk), a programme funded externally to deliver rehabilitative services with offenders in custody and the community. Approx. 600 users (100 concurrent) from non-government organisations use CATS to record work performed with offenders, creating an evidence base that supports performance management, payments to providers, ongoing research and audits from external bodies.

It is built using [ASP.NET](https://dotnet.microsoft.com/en-us/apps/aspnet) (for cross-platform development), and relies on the following technologies:

- C#
- Blazor Server & [MudBlazor](https://mudblazor.com/) (UI framework & components)
- Entity Framework Core & Microsoft SQL Server, with [SQL Project](https://learn.microsoft.com/en-us/sql/tools/sql-database-projects/sql-database-projects) for schema management
- [Quartz](https://www.quartz-scheduler.net/) (background jobs)
- [Rebus](https://github.com/rebus-org/Rebus) with RabbitMQ (messaging/outbox)
- [.NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/) (local orchestration)

## External dependencies

The solution depends on the [CFO Data Management System (DMS)](https://github.com/ministryofjustice/CFO-DataManagementSystem) for information sourced from the National Offender Management Information System (NOMIS) via Offloc, and nDelius via the cfoextract. DMS aggregates this information and exposes it to CATS via the DMS API.

## Documentation

- [Run locally for training](docs/local-training.md)
- [Build, test and publish with Cake](docs/build.md)
- [Contributing](CONTRIBUTING.md)
