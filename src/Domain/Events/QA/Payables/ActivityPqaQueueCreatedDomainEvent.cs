﻿using Cfo.Cats.Domain.Common.Events;
using Cfo.Cats.Domain.Entities.Activities;

namespace Cfo.Cats.Domain.Events.QA.Payables
{
    public sealed class ActivityPqaQueueCreatedDomainEvent(ActivityPqaQueueEntry entity) : CreatedDomainEvent<ActivityPqaQueueEntry>(entity)
    {
    }
}