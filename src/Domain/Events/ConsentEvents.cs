using Cfo.Cats.Domain.Common.Events;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Domain.Events;


public sealed class ConsentCreatedDomainEvent(Consent entity, string participantId, DateTime consentDate) : CreatedDomainEvent<Consent>(entity)
{
    public string ParticipantId { get; set; } = participantId;
    public DateTime ConsentDate { get; set; } = consentDate;
}