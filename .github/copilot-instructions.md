# Copilot Instructions

## Overview

CATS (Case Assessment and Tracking System) is a .NET 10 Blazor Server application for HMPPS Creating Future Opportunities (CFO). It tracks rehabilitative work with offenders in custody and the community, supporting ~600 users from non-government organisations.

External offender data (from Nomis and Delius) is sourced via the [CFO External Data Integration System](https://github.com/ministryofjustice/CFO-ExternalDataIntegrationSystem).

---

## Build, Test & Run

```bash
# Run all tests
dotnet cake.cs --target=Test

# Run a single test project
dotnet test test/Application.UnitTests/Application.UnitTests.csproj

# Run a single test by name
dotnet test test/Application.UnitTests/Application.UnitTests.csproj --filter "FullyQualifiedName~AddLabelCommandTests"

# Build only
dotnet cake.cs --target=Build

# Full publish (Clean → Restore → Build → Test → Publish)
dotnet cake.cs
```

Run the application via .NET Aspire — use the `Cats.AppHost` project (F5 in VS Code with the default configuration, or select `Cats.AppHost` in VS/Rider).

---

## Architecture

The solution follows **Clean Architecture** with four main layers:

- **`Domain`** — Entities, value objects, business rules, domain events, enums. No dependencies on other layers.
- **`Application`** — CQRS (MediatR), validators (FluentValidation), DTOs, AutoMapper profiles, and integration event contracts. Depends only on Domain.
- **`Infrastructure`** — EF Core (SQL Server), Quartz background jobs, Rebus message bus (outbox), Amazon S3, identity, and external service integrations. Implements Application interfaces.
- **`Server.UI`** — Blazor Server UI using MudBlazor. Pages use a code-behind pattern (`.razor` + `.razor.cs`).

Supporting projects:
- `src/Aspire/Cats.AppHost` — Aspire orchestration host for local dev and Kubernetes publishing.
- `src/DatabaseSeeding` — standalone seeder used during deployment.

---

## Key Conventions

### CQRS & MediatR

Every command/query (`IRequest<T>`) **must** have either `[RequestAuthorize(Policy = SecurityPolicies.XYZ)]` or `[AllowAnonymous]`. This is enforced by an architecture test (`RequestTests.Commands_Should_HaveAuthorizeAttribute`).

Security policies are defined in `Application/SecurityConstants/SecurityPolicies.cs`. Roles in `RoleNames.cs`.

```csharp
[RequestAuthorize(Policy = SecurityPolicies.SeniorInternal)]
public class AddLabelCommand : IRequest<Result> { ... }
```

Commands return `Result` or `Result<T>`. Use `Result.Success()`, `Result.Failure(...)`, `Result<T>.Success(data)`.

### Handlers & Data Access

Architecture test `HandlersDoNotReferToDbContextDirectly` enforces that **handlers must not inject `IApplicationDbContext` or `ApplicationDbContext` directly**. Use:
- Domain-specific repositories (e.g., `ILabelRepository`)
- `IUnitOfWork` (exposes `DbContext` and transaction management) for cross-entity operations

### Business Rules

Entities extend `BaseEntity<TId>` and enforce invariants via `CheckRule(IBusinessRule)`, which throws `BusinessRuleValidationException`. Implement `IBusinessRule` with `IsBroken()` and `Message`.

```csharp
protected void CheckRule(IBusinessRule rule)  // throws if rule.IsBroken()
```

Business rules live alongside the entity or feature they protect (e.g., `Domain/Labels/`, `Application/Features/Labels/BusinessRules/`).

### Domain Events & Integration Events

- Domain events: raised on entities via `AddDomainEvent(...)`, handled in Application via MediatR `INotificationHandler<>`.
- Integration events: written to the outbox via `context.InsertOutboxMessage(message)` and published to the Rebus message bus by `PublishOutboxMessagesJob`. Handlers live in `Application/Features/{Feature}/IntegrationEvents/` and `IntegrationEventHandlers/`.

### Multi-Tenancy & Auditing

- Entities implementing `IMustHaveTenant` or `IMayHaveTenant` get `TenantId` auto-populated by the `AuditableEntityInterceptor` EF interceptor.
- Entities implementing `IAuditable` (via `BaseAuditableEntity`) get `CreatedBy`, `Created`, `LastModifiedBy`, `LastModified` auto-set.
- Tenant IDs use a hierarchical dot-notation format, e.g. `1.2.3.` (regex: `^(\d+(\.\d+)*\.)$`).

### Validators

FluentValidation validators live alongside their command, named `{Command}Validator`. Use regex constants from `Application/Common/Validators/ValidationConstants.cs` rather than inline patterns.

```csharp
RuleFor(v => v.Name)
    .Matches(ValidationConstants.LettersSpacesUnderscores)
    .WithMessage(string.Format(ValidationConstants.LettersSpacesUnderscoresMessage, "Name"));
```

### MediatR Pipeline Behaviours (order matters)

Registered in `Application/DependencyInjection.cs`:
1. `TraceMetricsBehaviour` — Sentry performance tracing
2. `ValidationBehaviour` — runs FluentValidation validators
3. `UnhandledExceptionBehaviour` — catches and logs unhandled exceptions
4. `RequestExceptionProcessorBehavior`
5. `SessionValidatingBehaviour`
6. `AuthorizationBehaviour` — checks `[RequestAuthorize]` policy
7. `TransactionBehaviour` — wraps commands in a DB transaction
8. `AccessAuditingBehaviour` — records participant access audit trails (for `IAuditableRequest`)

### Blazor UI Patterns

- Pages use code-behind: `Foo.razor` + `Foo.razor.cs` (partial class).
- `_Imports.cs` and `_Imports.razor` in each layer/folder provide global using directives — new types rarely need explicit `using` statements.
- MudBlazor components are used throughout for UI.

### Feature Folder Structure

Features follow a consistent structure under `Application/Features/{Feature}/`:
```
Commands/
  {CommandName}/
    {CommandName}Command.cs
    {CommandName}CommandHandler.cs
    {CommandName}CommandValidator.cs
DTOs/
EventHandlers/
IntegrationEvents/
IntegrationEventHandlers/
Queries/
Specifications/
```

### Background Jobs (Quartz)

Jobs live in `Infrastructure/Jobs/`. Key jobs: `PublishOutboxMessagesJob`, `SyncParticipantsJob`, `ArchiveParticipantsJob`, `GenerateOutcomeQualityDipSamplesJob`, `DisableDormantAccountsJob`.

### Tests

- `test/Application.UnitTests` — unit tests for commands, business rules, domain entities, and mapping profiles. Uses NUnit + Shouldly.
- `test/ArchitectureTests` — architecture fitness tests using NetArchTest. Run these locally to catch convention violations before CI.
