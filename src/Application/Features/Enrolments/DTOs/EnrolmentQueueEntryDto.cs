using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Enrolments.DTOs;

public class EnrolmentQueueEntryDto
{
    public string ParticipantId { get; set; } = default!; 
    public DateTime Created { get; set; } = default!;
    public string TenantId { get; set; } = default!;
    public string TenantName { get; set; } = default!;
    public string ParticipantName { get; set; } = default!;

    public string SupportWorker { get; set; } = default!;

    public NoteDto[] Notes { get; set; } = [];

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<EnrolmentPqaQueueEntry, EnrolmentQueueEntryDto>()
                .ForMember(target => target.ParticipantId,
                options => options.MapFrom(source => source.ParticipantId))
                .ForMember(target => target.Created,
                options => options.MapFrom(source => source.Created))
                .ForMember(target => target.TenantId,
                options => options.MapFrom(source => source.TenantId))
                .ForMember(target => target.TenantName,
                options => options.MapFrom(source => source.Tenant!.Name))
                .ForMember(target => target.ParticipantName, options => {
                    options.MapFrom(target => target.Participant!.FirstName + " " + target.Participant.LastName);
                })
                .ForMember(target => target.SupportWorker, options => options.MapFrom(
                    source => source.Participant!.Owner!.DisplayName
                ))
                .ForMember(target => target.Notes, options => options.MapFrom(source => source.Notes));
            
            CreateMap<EnrolmentQa1QueueEntry, EnrolmentQueueEntryDto>()
                .ForMember(target => target.ParticipantId,
                options => options.MapFrom(source => source.ParticipantId))
                .ForMember(target => target.Created,
                options => options.MapFrom(source => source.Created))
                .ForMember(target => target.TenantId,
                options => options.MapFrom(source => source.TenantId))
                .ForMember(target => target.TenantName,
                options => options.MapFrom(source => source.Tenant!.Name))
                .ForMember(target => target.ParticipantName, options => {
                    options.MapFrom(target => target.Participant!.FirstName + " " + target.Participant.LastName);
                })
                .ForMember(target => target.SupportWorker, options => options.MapFrom(
                source => source.Participant!.Owner!.DisplayName
                ))
                .ForMember(target => target.Notes, options => options.MapFrom(source => source.Notes));
            
        }
    }

}

