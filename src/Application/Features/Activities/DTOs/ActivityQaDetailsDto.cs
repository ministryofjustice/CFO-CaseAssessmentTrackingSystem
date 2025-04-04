using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Domain.Entities.Activities;
using Cfo.Cats.Domain.Entities.Documents;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Activities.DTOs;

public class ActivityQaDetailsDto
{
    public Guid? ActivityId { get; set; }

    public required Guid TaskId { get; set; }
    public required DateTime Created { get; set; }
    public required ActivityStatus Status { get; set; }     
    
    public required string ParticipantId { get; set; }
    
    public Participant? Participant { get; set; }
    //public required Guid TaskId { get; set; }

    [Description("Location")]
    public LocationDto? Location { get; set; }

    [Description("Activity/ETE")]
    public ActivityDefinition? Definition { get; set; }

    [Description("Date activity took place")]
    public DateTime? CommencedOn { get; set; }

    [Description("Additional Information")]
    public string? AdditionalInformation { get; set; }

    public EmploymentDto EmploymentTemplate { get; set; } = new();
    public EducationTrainingDto EducationTrainingTemplate { get; set; } = new();
    public IswDto ISWTemplate { get; set; } = new();

    [Description("Upload Template")]
    public Document? Document { get; set; }

    class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<EmploymentActivity, EmploymentDto>();

            CreateMap<EducationTrainingActivity, EducationTrainingDto>();

            CreateMap<ISWActivity, IswDto>();

            CreateMap<Activity, ActivityQaDetailsDto>()
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.TookPlaceAtLocation))
                .ForMember(dest => dest.ActivityId, opt => opt.MapFrom(src => src.Id))
                .ForPath(dest => dest.EmploymentTemplate, opt => opt.MapFrom(src => src as EmploymentActivity))
                .ForPath(dest => dest.EducationTrainingTemplate, opt => opt.MapFrom(src => src as EducationTrainingActivity))
                .ForPath(dest => dest.ISWTemplate, opt => opt.MapFrom(src => src as ISWActivity))
                .ForPath(dest => dest.Document, opt => opt.MapFrom(src => (src as ActivityWithTemplate)!.Document));
        }
    }
}