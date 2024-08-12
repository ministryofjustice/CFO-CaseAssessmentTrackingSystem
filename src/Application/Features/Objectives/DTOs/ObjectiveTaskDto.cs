using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Objectives.DTOs;

public class ObjectiveTaskDto
{
    public required Guid Id { get; set; }
    public required Guid ObjectiveId { get; set; }
    public required string Title { get; set; }
    public required DateTime Due { get; set; }
    public required DateTime Created { get; set; }
    public DateTime? Completed { get; set; }
    public string? CompletedByName { get; set; }

    public bool IsCompleted => Completed.HasValue;
    public bool IsOverdue => IsCompleted is false && ToFirstDayOfMonth(DateTime.UtcNow) > ToFirstDayOfMonth(Due);
    public bool IsDueSoon => IsCompleted is false && ToFirstDayOfMonth(DateTime.UtcNow).Equals(ToFirstDayOfMonth(Due));

    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ObjectiveTask, ObjectiveTaskDto>()
                .ForMember(target => target.CompletedByName, options => options.MapFrom(source => source.CompletedByUser!.DisplayName));
        }
    }

    /// <summary>
    /// Takes a <paramref name="dateTime"/>
    /// </summary>
    private static DateTime ToFirstDayOfMonth(DateTime dateTime) => new(dateTime.Year, dateTime.Month, 1);

}
