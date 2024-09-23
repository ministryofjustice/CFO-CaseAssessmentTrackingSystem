using Cfo.Cats.Domain.Entities.Inductions;

namespace Cfo.Cats.Application.Features.Inductions.DTOs;

public record HubInductionDto(string ParticipantId, string HubName, DateTime InductionDate, string InductedBy)
{
    private class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<HubInduction, HubInductionDto>(MemberList.None)
                .ForCtorParam(nameof(ParticipantId), options => options.MapFrom(source => source.ParticipantId))
                .ForCtorParam(nameof(HubName), options => options.MapFrom(source => source.Location!.Name))
                .ForCtorParam(nameof(InductionDate), options => options.MapFrom(source => source.InductionDate))
                .ForCtorParam(nameof(InductedBy), options => options.MapFrom(source => source.Owner!.DisplayName));
        }
    }
}
