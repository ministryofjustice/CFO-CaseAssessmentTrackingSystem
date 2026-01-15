using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Common.Events;
using Cfo.Cats.Domain.Entities.Activities;

namespace Cfo.Cats.Domain.Events;

public sealed class ActivityFeedbackCreatedDomainEvent(ActivityFeedback entity) : CreatedDomainEvent<ActivityFeedback>(entity);
