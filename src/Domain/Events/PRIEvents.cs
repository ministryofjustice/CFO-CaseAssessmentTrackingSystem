namespace Cfo.Cats.Domain.Events;

public sealed class PRICreatedDomainEvent(string participantId, DateTime expectedReleaseDate) : DomainEvent
{
    public string ParticipantId { get; set; } = participantId;
    public DateTime ExpectedReleaseDate = expectedReleaseDate;
}