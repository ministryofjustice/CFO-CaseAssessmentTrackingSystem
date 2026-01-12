using Cfo.Cats.Application.Features.Documents.DTOs;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.DTOs;

public class ConsentDto
{
    public DateTime ConsentDate { get; set; }
    public Guid? DocumentId { get; set; } 
    
    public DateTime Created { get; set; }
    
    public string? FileName { get; set; }
    
    public string? Version { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Consent, ConsentDto>()
                .ForMember(c => c.DocumentId, options => options.MapFrom(source => source.Document!.Id))
                .ForMember(c => c.FileName, options => options.MapFrom(source => source.Document!.Title))
                .ForMember(c => c.ConsentDate, options => options.MapFrom(source => source.Lifetime.StartDate))
                .ForMember(c => c.Created, options => options.MapFrom(source => source.Created!))
                .ForMember(c => c.Version, options => options.MapFrom(source => source.Document!.Version));
        }
    }
}
