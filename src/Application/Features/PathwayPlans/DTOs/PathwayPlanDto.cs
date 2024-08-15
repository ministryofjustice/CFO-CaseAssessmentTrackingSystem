using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.PathwayPlans.DTOs;

public class PathwayPlanDto
{
    public required Guid Id { get; set; }
    public required string ParticipantId { get; set; }
    public required DateTime Created { get; set; }
    public required string CreatedBy { get; set; }
    public IEnumerable<ObjectiveDto> Objectives { get; set; } = [];

    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<PathwayPlan, PathwayPlanDto>();
        }

    }
}
