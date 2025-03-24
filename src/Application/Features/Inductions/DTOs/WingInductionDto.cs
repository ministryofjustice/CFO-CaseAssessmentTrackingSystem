using Cfo.Cats.Domain.Entities.Inductions;
using System.Linq;

namespace Cfo.Cats.Application.Features.Inductions.DTOs;

public record WingInductionDto(Guid Id, string ParticipantId, string WingName, DateTime InductionDate, string InductedBy, InductionPhase[] Phases)
{
    private class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<WingInduction, WingInductionDto>(MemberList.None)
                .ForCtorParam(nameof(Id), options => options.MapFrom(source => source.Id))
                .ForCtorParam(nameof(ParticipantId), options => options.MapFrom(source => source.ParticipantId))
                .ForCtorParam(nameof(WingName), options => options.MapFrom(source => source.Location!.Name))
                .ForCtorParam(nameof(InductionDate), options => options.MapFrom(source => source.InductionDate))
                .ForCtorParam(nameof(InductedBy), options => options.MapFrom(source => source.Owner!.DisplayName))
                .ForCtorParam(nameof(Phases), options => options.MapFrom(source =>
                    source.Phases
                      .OrderBy(p => p.StartDate) 
                      .ThenBy(p => p.CompletedDate) 
                      .ToArray()));
        }
    }
}