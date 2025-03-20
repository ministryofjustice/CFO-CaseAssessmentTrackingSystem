using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.Inductions;

public class WingInduction : OwnerPropertyEntity<Guid>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private WingInduction()
    { 
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private WingInduction(string participantId, int locationId, DateTime inductionDate, string ownerId)
    {
        Id = Guid.CreateVersion7();
        ParticipantId = participantId;
        LocationId = locationId;
        InductionDate = inductionDate;
        OwnerId = ownerId;

        AddDomainEvent(new WingInductionCreatedDomainEvent(this));
    }

    private List<InductionPhase> _phases = new();

    public IReadOnlyCollection<InductionPhase> Phases => _phases.AsReadOnly();

    public WingInduction AddPhase(DateTime startDate)
    {
        var previous = _phases.MaxBy(x => x.Number);
        
        if(previous is null)
        {
            _phases.Add(new InductionPhase(1, startDate, null, WingInductionPhaseStatus.Commenced, null, null,null));
        }
        else if (previous is { CompletedDate: null })
        {
            throw new ApplicationException("Previous phase must be completed");
        }
        else if (startDate < previous.CompletedDate)
        {
            throw new ApplicationException("Cannot start a new phase before the date of the previous phase");
        }
        else
        {
            _phases.Add(new InductionPhase(previous.Number + 1, startDate, null, WingInductionPhaseStatus.Commenced, null, null, null));
        }
        return this;
    }

    public WingInduction CompleteCurrentPhase(DateTime completionDate, string? completedBy)
    {
        var current = _phases.SingleOrDefault(x => x.CompletedDate is null);

        if (current is not null)
        {
            if (completionDate < current.StartDate)
            {
                throw new ApplicationException("Cannot start a complete phase before the date it commenced");
            }

            _phases.Remove(current);
            var phase = new InductionPhase(current.Number, current.StartDate, completionDate, WingInductionPhaseStatus.Completed, null, null, completedBy);
            _phases.Add(phase);
            AddDomainEvent(new InductionPhaseCompletedDomainEvent(Id, phase));
        }
        return this;
    }

    public WingInduction AbandonCurrentPhase(DateTime abandonDate, string? abandonJustification, WingInductionPhaseAbandonReason? abandonReason, string abandonedBy)
    {
        var current = _phases.SingleOrDefault(x => x.CompletedDate is null);

        if (current is not null)
        {
            if (abandonDate < current.StartDate)
            {
                throw new ApplicationException("Cannot abandon phase before the date it commenced");
            }

            _phases.Remove(current);
            var phase = new InductionPhase(current.Number, current.StartDate, abandonDate, WingInductionPhaseStatus.Abandoned, abandonJustification, abandonReason, abandonedBy);
            _phases.Add(phase);
            AddDomainEvent(new InductionPhaseCompletedDomainEvent(Id, phase));
        }
        return this;
    }

    public static WingInduction Create(string participantId, int locationId, DateTime inductionDate, string ownerId)
           => new(participantId, locationId, inductionDate, ownerId);

    public string ParticipantId { get; private set; }

    public int LocationId { get; private set; }

    public DateTime InductionDate { get; set; }

    public Location? Location { get; set; }

    /// <summary>
    /// Returns the open phase, will throw an exception if called when no open phases are
    /// available or more than one (should not happen)
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ApplicationException">If there isn't an open phase</exception>
    /// <exception cref="InvalidOperationException">If there are more than one open phases</exception>
    public InductionPhase GetOpenPhase()
    {
        return _phases.SingleOrDefault(c => c.CompletedDate is null)
               ?? throw new ApplicationException("Call to GetOpenPhase when no phases are open");
    }
}
