using Cfo.Cats.Application.Features.Locations.DTOs;

namespace Cfo.Cats.Application.Features.PRIs.DTOs;

[Description("PRIs")]
public class PRIDto
{
    [Description("PRI Id")]
    public required Guid Id { get; set; } = default!;

    [Description("Participant Id")]
    public required string ParticipantId { get; set; }

    [Description("Expected Date Of Release")]
    public required DateOnly ExpectedReleaseDate { get; set; }

    [Description("Actual Date Of Release")]
    public DateOnly? ActualReleaseDate { get; set; }

    public DateTime? AcceptedOn { get; set; }

    public required LocationDto ExpectedReleaseRegion { get; set; }
    public required LocationDto CustodyLocation { get; set; }

    public string? AssignedTo { get; set; }

    public PriStatus? Status { get; }
    public DateOnly MeetingAttendedOn { get; set; }
    public string? ReasonParticipantDidNotAttendInPerson { get; set; }
    public string? ReasonCommunityDidNotAttendInPerson { get; set; }
    public string? ReasonCustodyDidNotAttendInPerson { get; set; }
    public string? PostReleaseCommunitySupportInformation { get; set; }
    public bool CustodyAttendedInPerson => string.IsNullOrEmpty(ReasonCustodyDidNotAttendInPerson);
    public bool CommunityAttendedInPerson => string.IsNullOrEmpty(ReasonCommunityDidNotAttendInPerson);
    public bool ParticipantAttendedInPerson => string.IsNullOrEmpty(ReasonParticipantDidNotAttendInPerson);

    private class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Domain.Entities.PRIs.PRI, PRIDto>(MemberList.None)
             .ForMember(target => target.ExpectedReleaseRegion,
                options => options.MapFrom(source => source.ExpectedReleaseRegion))
             .ForMember(target => target.CustodyLocation,
                options => options.MapFrom(source => source.CustodyLocation))
          .ForMember(target => target.Status,
              options => options.MapFrom(source => source.Status));
        }
    }
}
