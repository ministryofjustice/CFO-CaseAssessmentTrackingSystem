using Cfo.Cats.Domain.Entities.Payables;

namespace Cfo.Cats.Application.Features.Payables.DTOs;

public class ActivitySummaryDto
{
    public required Guid Id { get; set; }
    public required DateTime Created { get; set; }
    public required DateTime Completed { get; set; }
    public required ActivityDefinition Definition { get; set; }
    public required ActivityStatus Status { get; set; }

    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Activity, ActivitySummaryDto>();
        }
    }

}
