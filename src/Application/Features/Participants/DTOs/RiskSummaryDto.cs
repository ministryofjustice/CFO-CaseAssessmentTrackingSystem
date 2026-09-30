using System.ComponentModel.DataAnnotations;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.DTOs;

public class RiskSummaryDto
{
    public required string ParticipantId { get; init; }

    [Description("Review Justification")]
    [MaxLength(5000)]
    public string? ReviewJustification { get; set; }
    public required RiskReviewReason ReviewReason { get; init; }
    public required DateTime Created { get; init; }
    public required string CreatedBy { get; init; }

    [Description("Date Risk form completed")]
    public DateTime? ReferredOn { get; init; }

    private class Mapping : Profile
    {
        public Mapping() => CreateMap<Risk, RiskSummaryDto>(MemberList.None);
    }
}

public class RiskUpdateDto
{
    /// <summary>
    /// The date and time the risk update was received from HMPPS.
    /// </summary>
    public required DateTime DateTime { get; init; }

    /// <summary>
    /// The HMPPS identifier the update relates to.
    /// </summary>
    public required string HmppsId { get; init; }
}