using Cfo.Cats.Domain.Entities.PRIs;

namespace Cfo.Cats.Application.Features.Participants.DTOs;

public class PriSummaryDto
{
    public required string ParticipantId { get; set; }

    public required DateTime Created { get; set; }
    public required string CreatedBy { get; set; }

    public DateOnly? ActualReleaseDate {  get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<PRI, PriSummaryDto>(MemberList.None);
        }
    }

}

