﻿using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.QualityAssurance.Queries;

public class EnrolmentPqaQueueEntrySpecification : Specification<EnrolmentPqaQueueEntry>
{
    public EnrolmentPqaQueueEntrySpecification(QueueEntryFilter filter)
    {
        Query.Where(e => e.TenantId
                .StartsWith(filter.CurrentUser!.TenantId!))
            .Where(e => e.IsCompleted == false);

        Query.Where(e => e.ParticipantId.Contains(filter.Keyword!), string.IsNullOrWhiteSpace(filter.Keyword) == false);
    }
}
