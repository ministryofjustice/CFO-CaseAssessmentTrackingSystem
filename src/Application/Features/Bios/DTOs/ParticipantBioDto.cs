using Cfo.Cats.Domain.Entities.Bios;

namespace Cfo.Cats.Application.Features.Bios.DTOs;

public class ParticipantBioDto
{
    public required string ParticipantId { get; set; } 
    public required DateTime CreatedDate { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ParticipantBio, ParticipantBioDto>()
                .ForMember(p => p.CreatedDate, options => options.MapFrom(source => source.Created));
        }
    }
}

