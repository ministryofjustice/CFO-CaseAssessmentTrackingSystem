using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Domain.Entities.PRIs;

namespace Cfo.Cats.Application.Features.PRIs.DTOs;

public class PRIDto
{
    public required string ParticipantId { get; set; }
    public required DateOnly ExpectedReleaseDate { get; set; }
    public required LocationDto ExpectedReleaseRegion { get; set; }
    public bool IsCompleted { get; }
    public DateOnly MeetingAttendedOn { get; set; }
    public bool MeetingAttendedInPerson { get; set; }
    public string? MeetingNotAttendedInPersonJustification { get; set; }

    class Mapping : Profile
    {
        Mapping()
        {
            CreateMap<PRI, PRIDto>();
        }
    }
}
