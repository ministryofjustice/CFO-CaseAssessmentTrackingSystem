using Cfo.Cats.Application.Outbox;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEvents;

/// <summary>
/// Raised when a user exercises an "area of concern" we want to capture usage for.
/// Published directly to the message bus (not via the outbox) because the odd lost
/// telemetry message is acceptable and we never want to replay it.
/// </summary>
public record UsageTrackedIntegrationEvent(
    string Area,
    string Activity,
    string? UserId,
    string? UserName,
    string? TenantId,
    string? Context,
    DateTime OccurredOn) : IntegrationEvent(OccurredOn);
