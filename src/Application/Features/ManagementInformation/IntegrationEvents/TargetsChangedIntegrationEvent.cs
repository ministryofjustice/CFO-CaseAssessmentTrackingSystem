using Cfo.Cats.Application.Outbox;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEvents;

public record TargetsChangedIntegrationEvent() : IntegrationEvent(DateTime.Now);