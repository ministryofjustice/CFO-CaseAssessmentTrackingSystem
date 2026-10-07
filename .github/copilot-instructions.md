# Copilot Instructions

## Architecture

The solution follows **Clean Architecture** with four main layers:

- **`Domain`** — Entities, value objects, business rules, domain events, enums. No dependencies on other layers.
- **`Application`** — CQRS (Cortex.Mediator), validators (FluentValidation), DTOs, AutoMapper profiles, and integration event contracts. Depends only on Domain.
- **`Infrastructure`** — EF Core (SQL Server), Quartz background jobs, Rebus message bus (outbox), Amazon S3, identity, and external service integrations. Implements Application interfaces.
- **`Server.UI`** — Blazor Server UI using MudBlazor. Pages use a code-behind pattern (`.razor` + `.razor.cs`).

Supporting projects:
- `src/Aspire/Cats.AppHost` — Aspire orchestration host for local dev and Kubernetes publishing.
- `src/DatabaseSeeding` — standalone seeder used during deployment.

---

## Key Conventions

### Core Framework Guardrails
- **CQRS Engine:** Use `Cortex.Mediator` (`ICommand<T>`, `IQuery<T>`, `INotificationHandler<T>`). **DO NOT** use `MediatR`.
- **Testing Stack:** Unit tests use NUnit and `Shouldly` assertions (`test/Application.UnitTests`). Architecture tests use `NetArchTest` (`test/ArchitectureTests`). **DO NOT** use `FluentAssertions` or `Assert`.
- **UI Base Component:** Blazor components must inherit `CatsComponent<T>`. **DO NOT** inherit from the legacy `CatsComponentBase`.

### CQRS & Cortex.Mediator

Every command/query (`ICommand<T>` or `IQuery<T>`) **must** have either `[RequestAuthorize(Policy = SecurityPolicies.XYZ)]` or `[AllowAnonymous]`. This is enforced by an architecture test (`RequestTests.Commands_Should_HaveAuthorizeAttribute`).

Security policies are defined in `Application/SecurityConstants/SecurityPolicies.cs`. Roles in `RoleNames.cs`.

```csharp
[RequestAuthorize(Policy = SecurityPolicies.SeniorInternal)]
public class AddLabelCommand : ICommand<Result> { ... }
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

- Domain events: raised on entities via `AddDomainEvent(...)`, handled in Application via Cortex.Mediator `INotificationHandler<>`.
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

### Blazor UI Patterns

- Pages use code-behind: `Foo.razor` + `Foo.razor.cs` (partial class).
- Components that access data via mediator ect **must inherit `CatsComponent<T>`**. Move away from `CatsComponentBase`.
- `_Imports.cs` and `_Imports.razor` in each layer/folder provide global using directives — new types rarely need explicit `using` statements.
- MudBlazor components are used throughout for UI.

### Feature Folder Structure

Features follow a consistent structure under `Application/Features/{Feature}/`:
```text
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