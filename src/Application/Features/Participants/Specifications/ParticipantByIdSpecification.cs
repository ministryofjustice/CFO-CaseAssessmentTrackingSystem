using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.Specifications;

public class ParticipantByIdSpecification : Specification<Participant>
{
    public ParticipantByIdSpecification(string id)
    {
        Query.Where(p => p.Id == id);
    }
}
