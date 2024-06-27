using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.DTOs;

public class ParticipantDto
{
    [Description("CATS Identifier")]
    public required string Id { get; set; }
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public DateOnly? DateOfBirth { get; set; }

    [Description("Enrolment Status")]
    public EnrolmentStatus? EnrolmentStatus { get; set; }

    [Description("Consent Status")]
    public ConsentStatus? ConsentStatus { get;set; }

    [Description("Current Location")]
    public LocationDto CurrentLocation { get; set; } = default!;
    
    [Description("Enrolment Location")]
    public LocationDto? EnrolmentLocation { get; set; }
    
    [Description("Enrolment Justification Reason")]
    public string? EnrolmentLocationJustification { get; set; }

    public ConsentDto[] Consents { get; set; } = [];

    public RightToWorkDto[] RightToWorks { get; set; } = [];
    
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Participant, ParticipantDto>()
                .ForMember(target => target.CurrentLocation,
                    options => options.MapFrom(source => source.CurrentLocation))
                .ForMember(target => target.Consents,
                    options => options.MapFrom(source => source.Consents.ToArray()))
                .ForMember(target => target.RightToWorks,
                    options => options.MapFrom(source => source.RightToWorks.ToArray()));
        }
    }
}
