using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.DTOs;

public class ParticipantIdentifierDto
{
    [Description("Identifier Value")]
    public required string Value { get; set; }

    [Description("Identifier Type")]
    public required ExternalIdentifierType Type { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ExternalIdentifier, ParticipantIdentifierDto>(MemberList.None);
        }
    }
}
