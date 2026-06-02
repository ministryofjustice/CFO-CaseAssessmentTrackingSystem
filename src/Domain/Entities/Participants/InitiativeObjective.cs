using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Domain.Entities.Participants;

public class InitiativeObjective : BaseAuditableEntity<Guid>, IMustHaveTenant
{
#pragma warning disable CS8618
    private InitiativeObjective() { }
#pragma warning restore CS8618

    public Guid ObjectiveId { get; private set; }
    public Guid InitiativeId { get; private set; }
    public string ParticipantId { get; private set; }
    public string TenantId { get; set; } = null!;
    public Initiative Initiative { get; private set; }

    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }

    public static InitiativeObjective Create(Guid objectiveId, Guid initiativeId, string participantId, DateOnly startDate)
        => new() { Id = Guid.CreateVersion7(), ObjectiveId = objectiveId, InitiativeId = initiativeId, ParticipantId = participantId, StartDate = startDate };

    public void Update(Guid initiativeId, DateOnly startDate)
    {
        InitiativeId = initiativeId;
        StartDate = startDate;
    }

    public void Close(DateOnly endDate) => EndDate = endDate;
}
