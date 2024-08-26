using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.PathwayPlans.DTOs;

public class PathwayPlanSummaryDto
{
    public required DateTime Created { get; set; }
    public required string CreatedBy { get; set; }
    public DateTime? LastReviewed { get; set; }
    public string? LastReviewedBy { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<PathwayPlan, PathwayPlanSummaryDto>(MemberList.None)
                .ForMember(target => target.LastReviewed, options =>
                    options.MapFrom(source => source.ReviewHistories
                        .OrderByDescending(history => history.Created)
                        .Select(history => history.Created)
                        .FirstOrDefault()))
                .ForMember(target => target.LastReviewedBy, options =>
                    options.MapFrom(source => source.ReviewHistories
                        .OrderByDescending(history => history.Created)
                        .Select(history => history.CreatedBy)
                        .FirstOrDefault()));
        }
    }
}
