namespace Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

public class ParticipantDipSampleEducationAndTrainingActivityDto : ParticipantDipSampleActivityDto
{
    public required string CourseTitle { get; init; }
    public string? CourseUrl { get; init; }
    public required string CourseLevel { get; init; }
    public DateTime CourseCommencedOn { get; init; } 
    public DateTime? CourseCompletedOn { get; init; }
    public required CourseCompletionStatus CompletionStatus { get;init; }
    public required Guid DocumentId { get; init; }

    public override bool HasTemplate() => true;
}