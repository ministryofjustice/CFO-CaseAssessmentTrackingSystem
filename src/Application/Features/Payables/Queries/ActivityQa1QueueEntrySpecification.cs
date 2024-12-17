﻿using Cfo.Cats.Domain.Entities.Payables;

namespace Cfo.Cats.Application.Features.Payables.Queries
{
    public class ActivityQa1QueueEntrySpecification : Specification<ActivityQa1QueueEntry>
    {
        public ActivityQa1QueueEntrySpecification(ActivityQueueEntryFilter filter)
        {
            Query.Where(e => e.TenantId
                    .StartsWith(filter.CurrentUser!.TenantId!))
                .Where(e => e.IsCompleted == false);
        }
    }
}