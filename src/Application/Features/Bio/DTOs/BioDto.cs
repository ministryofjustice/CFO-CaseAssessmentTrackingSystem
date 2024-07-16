using Cfo.Cats.Domain.Entities.Bios;

namespace Cfo.Cats.Application.Features.Bio.DTOs;

public class BioDto
{
    public required string ParticipantId { get; set; } 
    public required DateTime CreatedDate { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Cfo.Cats.Domain.Entities.Bios.Bio, BioDto>()
                .ForMember(p => p.CreatedDate, options => options.MapFrom(source => source.Created));
        }
    }
}

