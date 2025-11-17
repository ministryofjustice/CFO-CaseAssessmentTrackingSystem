using Cfo.Cats.EventBus.Events;

namespace Cfo.Cats.Application.Features.QualityAssurance.IntegrationEvents;

public record EnrolmentApprovedAtQaIntegrationEvent(string ParticipantId, DateTime ApprovalDate) : IntegrationEvent;
