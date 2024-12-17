using Cfo.Cats.Domain.Entities.Payables;

namespace Cfo.Cats.Application.Features.Payables.DTOs;

public class ActivitySummaryDto
{
    public required Guid Id { get; set; }
    public required string TookPlaceAtLocationName { get; set; }
    public required Guid TaskId { get; set; }
    public required DateTime Created { get; set; }
    public required DateTime Completed { get; set; }
    public required ActivityDefinition Definition { get; set; }
    public required ActivityStatus Status { get; set; }

    [Description("Additional Information (optional)")]
    public string? AdditionalInformation { get; set; }

    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Activity, ActivitySummaryDto>()
                .ForMember(dest => dest.TookPlaceAtLocationName, opts => opts.MapFrom(src => src.TookPlaceAtLocation.Name));
        }
    }

}
