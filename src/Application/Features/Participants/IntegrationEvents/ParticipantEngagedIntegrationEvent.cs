using Cfo.Cats.EventBus.Events;

namespace Cfo.Cats.Application.Features.Participants.IntegrationEvents;

public sealed record ParticipantEngagedIntegrationEvent(
    string ParticipantId, 
    string Description, 
    string Category, 
    DateOnly EngagedOn,
    string EngagedAtLocation,
    string EngagedAtContract,
    string EngagedWith,
    string EngagedWithTenant) : IntegrationEvent;