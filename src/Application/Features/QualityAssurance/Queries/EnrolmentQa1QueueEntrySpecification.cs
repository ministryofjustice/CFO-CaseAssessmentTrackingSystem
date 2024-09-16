﻿using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.QualityAssurance.Queries;

public class EnrolmentQa1QueueEntrySpecification : Specification<EnrolmentQa1QueueEntry>
{
    public EnrolmentQa1QueueEntrySpecification(QueueEntryFilter filter)
    {
        Query.Where(e => e.TenantId
                .StartsWith(filter.CurrentUser!.TenantId!))
            .Where(e => e.IsCompleted == false);

        Query.Where(e => e.ParticipantId.Contains(filter.Keyword!), string.IsNullOrWhiteSpace(filter.Keyword) == false);
    }
}
