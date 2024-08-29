using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.DTOs;

public class ParticipantSupervisorDto
{
    [Description("Name")]
    public string? Name { get; set; }

    [Description("Telephone Number")]
    public string? TelephoneNumber { get; set; }

    [Description("Mobile Number")]
    public string? MobileNumber { get; set; }

    [Description("Email Address")]
    public string? EmailAddress { get; set; }

    [Description("Address")]
    public string? Address { get; set; }

    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Supervisor, ParticipantSupervisorDto>(MemberList.None)
                .ReverseMap();
        }
    }
}