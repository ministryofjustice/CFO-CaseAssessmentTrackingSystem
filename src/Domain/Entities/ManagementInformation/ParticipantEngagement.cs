using Cfo.Cats.Domain.Common.Entities;

namespace Cfo.Cats.Domain.Entities.ManagementInformation;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

public class ParticipantEngagement : BaseEntity<Guid>
{ 
    public ParticipantEngagement()
    {
        Id = Guid.CreateVersion7();
        CreatedOn = DateTime.UtcNow;
    }

    public string ParticipantId { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public DateOnly EngagedOn { get; set; }
    public string EngagedAtLocation { get; set; }
    public string EngagedAtContract { get; set; }
    public string EngagedWith { get; set; }
    public string EngagedWithTenant { get; set; }
    public DateTime CreatedOn { get; set; }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
