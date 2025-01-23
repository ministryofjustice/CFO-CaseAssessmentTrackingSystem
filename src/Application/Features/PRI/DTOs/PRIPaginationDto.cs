namespace Cfo.Cats.Application.Features.PRI.DTOs;

[Description("PRIs")]
public class PRIPaginationDto
{
    [Description("PRI Id")]
    public required Guid Id { get; set; } = default!;

    [Description("Participant Id")]
    public required string ParticipantId { get; set; }

    [Description("Actual Date Of Release")]
    public DateOnly? ActualReleaseDate { get; set; }


    [Description("Expected Date Of Release")]
    public required DateOnly ExpectedReleaseDate { get; set; }

    [Description("Community Support Worker")]
    public string? AssignedTo { get; private set; }

    [Description("Custody Support Worker")]
    public string? CreatedBy { get; set; }

    //public required LocationDto ExpectedReleaseRegion { get; set; }
    //public bool IsCompleted { get; }
    //public DateOnly MeetingAttendedOn { get; set; }
    //public bool MeetingAttendedInPerson { get; set; }
    //public string? MeetingNotAttendedInPersonJustification { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.PRIs.PRI, PRIPaginationDto>();
                //.ForMember(x => x.CreatedBy, s => s.MapFrom(y => y.CustodyWorker!.DisplayName));
                //.ForMember(target => target.CreatedBy, options => options.MapFrom(source => source.Owner!.DisplayName));
        }
    }

    //private class Mapper : Profile
    //{
    //    public Mapper()
    //    {
    //        CreateMap<PRIDto, PRIPaginationDto>(MemberList.None)
    //            .ForMember(target => target.PRIId, options => options.MapFrom(source => source.PRIId))
    //            .ForMember(target => target.ParticipantId, options => options.MapFrom(source => source.ParticipantId))
    //            .ForMember(target => target.ExpectedReleaseDate, options => options.MapFrom(source => source.ExpectedReleaseDate))
    //            .ForMember(target => target.ActualDateOfRelease, options => options.MapFrom(source => source.ActualDateOfRelease));
    //    }
    //}
}