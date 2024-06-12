using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.DTOs;

public class ParticipantDto
{
    [Description("CATS Identifier")]
    public string Id { get; set; }
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
    
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Participant, ParticipantDto>()
                .ForMember(target => target.CurrentLocation,
                options => options.MapFrom(source => source.CurrentLocation));
        }
    }
}
