using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.Inductions;

public class HubInduction : OwnerPropertyEntity<Guid>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private HubInduction()
    { 
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private HubInduction(string participantId, int locationId, DateTime inductionDate, string ownerId)
    {
        Id = Guid.CreateVersion7();
        ParticipantId = participantId;
        LocationId = locationId;
        InductionDate = inductionDate;
        OwnerId = ownerId;
        
        AddDomainEvent(new HubInductionCreatedDomainEvent(this));
    }

    public static HubInduction Create(string participantId, int locationId, DateTime inductionDate, string ownerId) 
        => new(participantId, locationId, inductionDate, ownerId);

    public string ParticipantId { get; private set; }
    
    public int LocationId { get; private set; }

    public DateTime InductionDate { get; set; }
    
    public Location? Location { get; set; }
}
