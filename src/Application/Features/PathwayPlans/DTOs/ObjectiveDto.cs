using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.PathwayPlans.DTOs;

public class ObjectiveDto
{
    public required Guid Id { get; set; }
    public required Guid PathwayPlanId { get; set; }
    public required string Description { get; set; }
    public DateTime? Completed { get; set; }
    public string? CompletedBy { get; set; }
    public CompletionStatus? CompletedStatus { get; set; }
    public required DateTime Created { get; set; }
    public required string CreatedBy { get; set; }
    public IEnumerable<ObjectiveTaskDto> Tasks { get; set; } = [];
    public string? Justification { get; set; }
    public required int Index { get; set; }
    public string DisplayName => $"{Index}. {Description}";
    public bool IsCompleted => Completed.HasValue;

    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Objective, ObjectiveDto>();
        }
    }
}
