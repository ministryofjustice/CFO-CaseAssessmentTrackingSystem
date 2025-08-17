namespace Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

public class ParticipantDipSampleObjectiveTaskDto
{
    public required Guid Id { get; init; }
    public required int Index { get; init; }
    public required string Description { get; init; }
    public DateTime Due { get; init; }
    public string? Status { get; init; }
    public string? Justification { get; init; }
    public DateTime Created { get; init; }
    public required string CreatedBy { get; init; }
    public string? CompletedBy { get; init; }
    public DateTime? Completed { get; init; }

    public ParticipantDipSampleActivityDto[] Activities { get; set; } = [];
    
    public bool IsOverdue => Completed is null
                             && (ToFirstDayOfMonth(DateTime.UtcNow) > ToFirstDayOfMonth(Due));

    public bool IsDueSoon => Completed is null
                             && (ToFirstDayOfMonth(DateTime.UtcNow).Equals(ToFirstDayOfMonth(Due))
                                 || IsOverdue is false && (DateTime.UtcNow.AddDays(14) > ToFirstDayOfMonth(Due)));

    private static DateTime ToFirstDayOfMonth(DateTime dateTime) => new(dateTime.Year, dateTime.Month, 1);
}