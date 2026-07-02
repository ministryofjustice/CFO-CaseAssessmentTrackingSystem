namespace Cfo.Cats.Application.Features.QualityAssurance.DTOs;

public record ServiceDeskQueueSummaryDto
{
    public int EnrolmentQa1Count { get; init; }
    public int EnrolmentQa2Count { get; init; }
    public int EnrolmentEscalationCount { get; init; }
    public int ActivityQa1Count { get; init; }
    public int ActivityQa2Count { get; init; }
    public int ActivityEscalationCount { get; init; }
}