using Cfo.Cats.Domain.Entities.Activities;

namespace Cfo.Cats.Application.Features.Activities.DTOs;
#nullable disable
public class ActivityQaNoteDto
{
    public Guid ActivityId { get; set; }
    public required DateTime Created { get; set; }
    public required string Message { get; set; }
    public required string CreatedBy { get; set; }
    public required string TenantName { get; set; }
    public required bool IsExternal { get; set; }

    private class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<ActivityQueueEntryNote, ActivityQaNoteDto>()                
                .ForMember(target => target.CreatedBy, options => options.MapFrom(source => source.CreatedByUser.DisplayName))
                .ForMember(target => target.Message, options => options.MapFrom(source => source.Message))
                .ForMember(target => target.Created, options => options.MapFrom(source => source.Created))
                .ForMember(target => target.TenantName, options => options.MapFrom(source => source.CreatedByUser.TenantName));
        }
    }
}