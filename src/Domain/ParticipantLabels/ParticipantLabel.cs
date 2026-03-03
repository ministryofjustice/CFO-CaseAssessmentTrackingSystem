using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Exceptions;
using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Domain.ParticipantLabels.Events;
using Cfo.Cats.Domain.ParticipantLabels.Rules;
using Cfo.Cats.Domain.Participants;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Domain.ParticipantLabels;

public class ParticipantLabel : BaseAuditableEntity<ParticipantLabelId>, ILifetime
{
    // Temporary backing field required while Participant.Id remains a string in EF mappings.
    // This can be removed once Participant is migrated to use ParticipantId as its key type.
    private string _participantId = null!;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private ParticipantLabel()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    
    private ParticipantLabel(ParticipantId participantId, Label label, IParticipantLabelsCounter counter)
    {
        CheckRule(new LabelMustNotAlreadyBeOpen(participantId, label.Id, counter));
        
        Id = new ParticipantLabelId(Guid.CreateVersion7());
        
        _participantId = participantId.Value;
        Label = label;
        Lifetime = new Lifetime(DateTime.UtcNow, DateTime.MaxValue);
        
        AddDomainEvent(new ParticipantLabelCreatedDomainEvent(this));
    }

    /// <summary>
    /// Creates a new ParticipantLabel entity.
    /// </summary>
    /// <param name="participantId">The identifier of the participant.</param>
    /// <param name="label">The label to link to the participant.</param>
    /// <param name="counter">An implementation that will provide counts of open participants</param>
    /// <returns></returns>
    /// <exception cref="BusinessRuleValidationException">If the participant label is already open on the participant id</exception>
    public static ParticipantLabel Create(
        ParticipantId participantId,
        Label label, 
        IParticipantLabelsCounter counter
        ) => new (participantId, label, counter);
        
    /// <summary>
    /// Closes the label.
    ///
    /// System labels must be closed by forcing.
    /// </summary>
    /// <param name="force">Indicates the label should be forcibly closed, even if it is a system label.</param>
    /// <exception cref="BusinessRuleValidationException">If the participant label is as System scoped label and force has not been set to true</exception>
    public void Close(bool force = false)
    {
        CheckRule(new CannotCloseSystemLabelsWithoutForcing(Label,force));
        
        Lifetime = Lifetime.Close();
        AddDomainEvent(new ParticipantLabelClosedDomainEvent(this));
    }

    public ParticipantId ParticipantId => new(_participantId);
    
    public Label Label { get; private set; }
    public Lifetime Lifetime { get; private set; }
}