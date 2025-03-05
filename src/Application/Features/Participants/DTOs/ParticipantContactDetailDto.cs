using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.DTOs;

public class ParticipantContactDetailDto
{
    public required string Description { get; set; }
    public required string ParticipantId { get; set; }
    public required string Address { get; set; }
    public string? EmailAddress { get; set; }
    public string? PhoneNumber { get; set; }
    public required bool Primary { get; set; }
    public required DateTime Created { get; set; }

    class Mapping : Profile
    {
        public Mapping() => CreateMap<ParticipantContactDetail, ParticipantContactDetailDto>();
    }
}
