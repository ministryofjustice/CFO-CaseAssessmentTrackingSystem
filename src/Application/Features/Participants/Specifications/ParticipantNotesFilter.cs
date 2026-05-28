namespace Cfo.Cats.Application.Features.Participants.Specifications;

public class ParticipantNotesFilter : PaginationFilter
{
    public required string ParticipantId { get; init; }
}
