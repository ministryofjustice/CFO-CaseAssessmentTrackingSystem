using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.ExternalRisk;

public class ParticipantRiskUpdate : IEntity<int>
{

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private ParticipantRiskUpdate()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    public static ParticipantRiskUpdate Create(string participantId, string hmppsId, string eventType, DateTime dateTime)
        => new()
        {
            ParticipantId = participantId,
            HmppsId = hmppsId,
            EventType = eventType,
            DateTime = dateTime
        };
    
    public int Id { get; private set; }

    public string ParticipantId { get; private set; }

    public string HmppsId { get; private set; }

    /// <summary>The raw HMPPS event type, e.g. RISK_OF_SERIOUS_HARM_CHANGED.</summary>
    public string EventType { get; private set; }

    public DateTime DateTime { get; private set; }

    public IReadOnlyCollection<DomainEvent> DomainEvents => [];

    public void ClearDomainEvents() {}

}