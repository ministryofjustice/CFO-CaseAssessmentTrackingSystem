using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Application.Features.Participants.Specifications;

public class ParticipantNotesSpecification : Specification<Note>
{
    public ParticipantNotesSpecification() =>
        Query
            .AsNoTracking()
            .OrderByDescending(n => n.Created);
}
