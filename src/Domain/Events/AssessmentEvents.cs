using Cfo.Cats.Domain.Common.Events;
using Cfo.Cats.Domain.Entities.Assessments;

namespace Cfo.Cats.Domain.Events;

public sealed class AssessmentCreatedDomainEvent(ParticipantAssessment entity)
    : CreatedDomainEvent<ParticipantAssessment>(entity);

public sealed class AssessmentScoredDomainEvent(ParticipantAssessment entity)
    : UpdatedDomainEvent<ParticipantAssessment>(entity)
{
    
}