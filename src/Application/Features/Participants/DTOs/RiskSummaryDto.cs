using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.DTOs;

public class RiskSummaryDto
{
    public required string ParticipantId { get; set; }

    [Description("Review Justification")]
    public string? ReviewJustification { get; set; }
    public required RiskReviewReason ReviewReason { get; set; }
    public required DateTime Created { get; set; }
    public required string CreatedBy { get; set; }
    public DateTime? ReferredOn { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Risk, RiskSummaryDto>(MemberList.None);
        }
    }

}
