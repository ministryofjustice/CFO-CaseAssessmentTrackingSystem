using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.PathwayPlans.DTOs;

public class ObjectiveTaskDto
{
    public required Guid Id { get; init; }
    public required string Description { get; init; }
    public required Guid ObjectiveId { get; init; }
    public required DateTime Due { get; init; }
    public required DateTime Created { get; init; }
    public required string CreatedBy { get; init; }
    public DateTime? Completed { get; init; }
    public string? CompletedBy { get; init; }
    public CompletionStatus? CompletedStatus { get; init; }
    public required int Index { get; init; }
    public string? Justification { get; init; }
    public string DisplayName => $"{Index}. {Description}";
    public bool IsCompleted => Completed.HasValue;
    public bool IsOverdue => IsCompleted is false
        && (ToFirstDayOfMonth(DateTime.UtcNow) > ToFirstDayOfMonth(Due));

    public bool IsDueSoon => IsCompleted is false
        && (ToFirstDayOfMonth(DateTime.UtcNow).Equals(ToFirstDayOfMonth(Due))
        || IsOverdue is false && (DateTime.UtcNow.AddDays(14) > ToFirstDayOfMonth(Due)));

    public required bool IsMandatory { get; init; }
    public bool CanBeEdited => IsMandatory is false;
    public bool CanAddActivity => IsMandatory is false;

    public class Mapping : Profile
    {
        public Mapping() => CreateMap<ObjectiveTask, ObjectiveTaskDto>();
    }

    /// <summary>
    /// Takes a <paramref name="dateTime"/>
    /// </summary>
    private static DateTime ToFirstDayOfMonth(DateTime dateTime) => new(dateTime.Year, dateTime.Month, 1);

}
