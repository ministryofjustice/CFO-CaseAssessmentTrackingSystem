using Cfo.Cats.Application.Features.Locations.DTOs;

namespace Cfo.Cats.Application.Features.PRIs.DTOs;

[Description("PRIs")]
public class PRIDto
{
    [Description("PRI Id")]
    public required Guid Id { get; set; } = default!;

    [Description("Participant Id")]
    public required string ParticipantId { get; set; }

    [Description("Actual Date Of Release")]
    public DateOnly? ActualReleaseDate { get; set; }

    public DateTime? AcceptedOn { get; private set; }

    [Description("Expected Date Of Release")]
    public required DateOnly ExpectedReleaseDate { get; set; }

    public required LocationDto ExpectedReleaseRegion { get; set; }

    public string? AssignedTo { get; private set; }

    public bool IsCompleted { get; }

    private class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Domain.Entities.PRIs.PRI, PRIDto>(MemberList.None)
                .ForMember(target => target.Id, options => options.MapFrom(source => source.Id))
          .ForMember(target => target.ParticipantId,
              options => options.MapFrom(source => source.ParticipantId))
          .ForMember(target => target.ActualReleaseDate,
              options => options.MapFrom(source => source.ActualReleaseDate))
          .ForMember(target => target.ExpectedReleaseDate,
              options => options.MapFrom(source => source.ExpectedReleaseDate))
             .ForMember(target => target.ExpectedReleaseRegion,
                options => options.MapFrom(source => source.ExpectedReleaseRegion))
          .ForMember(target => target.IsCompleted,
              options => options.MapFrom(source => source.IsCompleted));
        }
    }
}
