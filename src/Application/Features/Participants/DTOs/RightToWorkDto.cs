using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.DTOs;

public class RightToWorkDto
{
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }

    public Guid? DocumentId { get; set; }

    public string? FileName { get; set; }

    public DateTime Created { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<RightToWork, RightToWorkDto>()
                .ForMember(rtw => rtw.DocumentId,
                    options => options.MapFrom(source => source.Document!.Id))
                .ForMember(c => c.FileName,
                    options => options.MapFrom(source => source.Document!.Title))
                .ForMember(c => c.ValidFrom,
                    options => options.MapFrom(source => source.Lifetime.StartDate))
                .ForMember(c => c.ValidTo,
                    options => options.MapFrom(source => source.Lifetime.EndDate))
                .ForMember(o => o.Created, 
                    options => options.MapFrom(source => source.Created));
        }
    }
}