using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.DTOs;

public class ExternalIdentifierDto
{
    public string Type {get; set;} = default!;
    public string Value {get;set;} = default!;
    
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ExternalIdentifier, ExternalIdentifierDto>(MemberList.None)
                .ForMember(x => x.Value, o => o.MapFrom(s => s.Value))
                .ForMember(x => x.Type, o => o.MapFrom(s => s.Type.Name));
        }
    }

}