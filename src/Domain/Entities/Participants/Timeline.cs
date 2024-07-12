using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Identity;

namespace Cfo.Cats.Domain.Entities.Participants;

public class Timeline : BaseAuditableEntity<int>
{
   
 #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Timeline()
    {
    }
    #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private Timeline(string participantId, TimelineEventType eventType, string createdBy, string line1, string? line2 = null, string? line3 = null)
    {
        ParticipantId = participantId;
        EventType = eventType;
        CreatedBy = createdBy;
        Line1 = line1;
        Line2 = line2;
        Line3 = line3;
    }
    
    public string ParticipantId { get; private set; }
    
    public TimelineEventType EventType { get; private set; }
    public string Line1 { get; private set; }
    public string? Line2 { get; private set; }
    public string? Line3 { get; private set; }

    public static Timeline CreateTimeline(string participantId, TimelineEventType eventType, string createdBy, string line1, string? line2, string? line3)
    {
        return new Timeline(participantId, eventType, createdBy, line1, line2, line3);
    }

    public virtual ApplicationUser? CreatedByUser { get; private set; }
}

