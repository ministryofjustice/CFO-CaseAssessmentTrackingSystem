using Cfo.Cats.Domain.Common.Events;
using Cfo.Cats.Domain.Entities.Assessments;

namespace Cfo.Cats.Domain.Events;

public sealed class ParticipantAssessmentCreatedDomainEvent(ParticipantAssessment entity)
    : CreatedDomainEvent<ParticipantAssessment>(entity);