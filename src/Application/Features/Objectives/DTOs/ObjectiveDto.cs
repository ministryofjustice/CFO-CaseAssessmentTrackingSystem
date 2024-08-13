using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Objectives.DTOs;

public class ObjectiveDto
{
    public required Guid Id { get; set; }
    public required string ParticipantId { get; set; }
    public required string Title { get; set; }
    public required DateTime Created { get; set; }
    public IEnumerable<ObjectiveTaskDto> Tasks { get; set; } = [];

    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Objective, ObjectiveDto>();
        }
    }
}
