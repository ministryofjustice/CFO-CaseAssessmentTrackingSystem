using Cfo.Cats.Domain.Common.Events;

namespace Cfo.Cats.Domain.ParticipantLabels.Events;

public sealed class ParticipantLabelCreatedDomainEvent(ParticipantLabel label) : CreatedDomainEvent<ParticipantLabel>(label)
{
    
}