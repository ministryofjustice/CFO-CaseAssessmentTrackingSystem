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
