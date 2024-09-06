using Cfo.Cats.Application.Features.Documents.DTOs;
using Cfo.Cats.Domain.Entities.Participants;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Cfo.Cats.Application.Features.Participants.DTOs;

public class ConsentDto
{
    public DateTime ConsentDate { get; set; }
    public Guid? DocumentId { get; set; } 
    
    public string? FileName { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Consent, ConsentDto>()
                .ForMember(c => c.DocumentId, options => options.MapFrom(source => source.Document!.Id))
                .ForMember(c => c.FileName, options => options.MapFrom(source => source.Document!.Title))
                .ForMember(c => c.ConsentDate, options => options.MapFrom(source => source.Lifetime.StartDate));
        }
    }
}
