namespace Cfo.Cats.Domain.Entities.Participants;

public class InitiativeObjective
{
#pragma warning disable CS8618
    private InitiativeObjective() { }
#pragma warning restore CS8618

    public Guid ObjectiveId { get; private set; }
    public Guid InitiativeId { get; private set; }
    public string ParticipantId { get; private set; }

    public static InitiativeObjective Create(Guid objectiveId, Guid initiativeId, string participantId)
        => new() { ObjectiveId = objectiveId, InitiativeId = initiativeId, ParticipantId = participantId };

    public void Update(Guid initiativeId) => InitiativeId = initiativeId;
}
