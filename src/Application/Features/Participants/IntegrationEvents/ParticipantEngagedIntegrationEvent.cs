using Cfo.Cats.Application.Outbox;

namespace Cfo.Cats.Application.Features.Participants.IntegrationEvents;

public record ParticipantEngagedIntegrationEvent(
    string ParticipantId, 
    string Description, 
    string Category, 
    DateOnly EngagedOn,
    string EngagedAtLocation,
    string EngagedAtLocationType,
    string EngagedAtContract,
    string EngagedWith,
    string EngagedWithTenant) : IntegrationEvent(EngagedOn.ToDateTime(TimeOnly.MinValue));