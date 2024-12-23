using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Common.Events;
using Cfo.Cats.Domain.Entities.Activities;

namespace Cfo.Cats.Domain.Events;

public sealed class ActivityCreatedDomainEvent(Activity entity) : CreatedDomainEvent<Activity>(entity);
public sealed class EducationTrainingActivityCreatedDomainEvent(EducationTrainingActivity entity) : CreatedDomainEvent<EducationTrainingActivity>(entity);
public sealed class EmploymentActivityCreatedDomainEvent(EmploymentActivity entity) : CreatedDomainEvent<EmploymentActivity>(entity);
public sealed class ISWActivityCreatedDomainEvent(ISWActivity entity) : CreatedDomainEvent<ISWActivity>(entity);
public sealed class NonISWActivityCreatedDomainEvent(NonISWActivity entity) : CreatedDomainEvent<NonISWActivity>(entity);
public sealed class ActivityTransitionedDomainEvent(Activity activity, ActivityStatus from, ActivityStatus to) : DomainEvent
{
    public Activity Item { get; } = activity;
    public ActivityStatus From { get; } = from;
    public ActivityStatus To { get; } = to;
}
