using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.DTOs;
public class RiskHistoryDto
{
    public required Guid Id { get; set; }
    public required string ParticipantId { get; set; }
    public RiskReviewReason? RiskReviewReason { get; set; }
    public required DateTime CreatedDate { get; set; }
    public required string CreatedBy { get; set; }
    public DateTime? Completed { get; set; }
    public string? CompletedBy { get; set; }
    public int LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public int DaysSinceLastReview { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Risk, RiskHistoryDto>()
                .ForMember(r => r.Id, options => options.MapFrom(source => source.Id))
                .ForMember(r => r.CreatedDate, options => options.MapFrom(source => source.Created))
                .ForMember(r => r.CreatedBy, options => options.MapFrom(source => source.CreatedBy))
                .ForMember(r => r.Completed, options => options.MapFrom(source => source.Completed))
                .ForMember(r => r.CompletedBy, options => options.MapFrom(source => source.CompletedBy))
                .ForMember(r => r.LocationId, options => options.MapFrom(source => source.LocationId))
                .ForMember(r => r.RiskReviewReason, options => options.MapFrom(source => source.ReviewReason))
                .ForMember(r => r.LocationName, options => options.Ignore())
                .ForMember(r => r.DaysSinceLastReview, options => options.Ignore());
        }
    }
}
