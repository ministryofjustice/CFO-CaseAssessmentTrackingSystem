using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.QualityAssurance.Specifications;

public class EnrolmentQa1QueueEntrySpecification : Specification<EnrolmentQa1QueueEntry>
{
    public EnrolmentQa1QueueEntrySpecification(QueueEntryFilter filter)
    {
        Query.Where(e => e.TenantId
                .StartsWith(filter.CurrentUser!.TenantId!))
            .Where(e => e.IsCompleted == false);

        Query.Where(e => 
            e.ParticipantId.Contains(filter.Keyword!) || 
            e.Participant!.FirstName.Contains(filter.Keyword!) ||
            e.Participant!.LastName.Contains(filter.Keyword!),
            !string.IsNullOrEmpty(filter.Keyword));
    }
}