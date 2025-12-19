using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.PathwayPlans.DTOs;

public class PathwayPlanDto
{
    public required Guid Id { get; set; }
    public required string ParticipantId { get; set; }
    public required DateTime Created { get; set; }
    public required string CreatedBy { get; set; }
    public DateTime? LastReviewed { get; set; }
    public string? LastReviewedBy { get; set; }
    public IEnumerable<ObjectiveDto> Objectives { get; set; } = [];

    public class Mapping : Profile
    {
        public Mapping() =>
            CreateMap<PathwayPlan, PathwayPlanDto>()
                .ForMember(target => target.LastReviewed, options =>
                    options.MapFrom(source => source.PathwayPlanReviews
                        .OrderByDescending(history => history.Created)
                        .Select(history => history.Created)
                        .FirstOrDefault()))
                .ForMember(target => target.LastReviewedBy, options =>
                    options.MapFrom(source => source.PathwayPlanReviews
                        .OrderByDescending(history => history.Created)
                        .Select(history => history.CreatedBy)
                        .FirstOrDefault()));
    }
}