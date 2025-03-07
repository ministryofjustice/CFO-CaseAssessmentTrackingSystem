using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.DTOs;

public class ParticipantContactDetailDto
{
    public required Guid Id { get; set; }
    public required string Description { get; set; }
    public required string ParticipantId { get; set; }
    [Description("Address")] public required string Address { get; set; }
    public required string PostCode { get; set; }
    public required string UPRN { get; set; }
    [Description("Email Address")] public string? EmailAddress { get; set; }
    [Description("Phone Number")] public string? MobileNumber { get; set; }
    public required bool Primary { get; set; }
    public required DateTime Created { get; set; }

    class Mapping : Profile
    {
        public Mapping() => CreateMap<ParticipantContactDetail, ParticipantContactDetailDto>();
    }
}
