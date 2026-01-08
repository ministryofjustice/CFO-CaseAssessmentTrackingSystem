using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.QualityAssurance.DTOs;

public class EnrolmentQueueEntryDto
{
    public Guid Id { get; init; }
    public string ParticipantId { get; init; } = null!; 
    public DateTime Created { get; init; }
    public string TenantId { get; init; } = null!;
    public string TenantName { get; init; } = null!;
    public string ParticipantName { get; init; } = null!;

    public string SupportWorker { get; init; } = null!;

    public string? AssignedTo { get; init; } 

    public bool IsCompleted { get; init; }
    public bool IsAccepted { get; init; }
    public NoteDto[] Notes { get; init; } = [];
    
    public int NoOfPreviousSubmissions { get; set; }

    public string? Qa1CompletedBy { get; set; }

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
                    source => source.SupportWorker!.DisplayName
                ))
                .ForMember(target => target.Notes, options => options.MapFrom(source => source.Notes))
                .ForMember(target => target.IsCompleted, options => options.MapFrom(source=> source.IsCompleted))
                .ForMember(target => target.IsAccepted, options => options.MapFrom(source=> source.IsAccepted))
                .ForMember(target => target.AssignedTo, options => options.MapFrom(source => source.Owner!.DisplayName))
                .ForMember(target => target.NoOfPreviousSubmissions, options => options.Ignore())
                .ForMember(d => d.Qa1CompletedBy, o => o.Ignore());
            
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
                    source => source.SupportWorker!.DisplayName
                ))
                .ForMember(target => target.Notes, options => options.MapFrom(source => source.Notes))
                 .ForMember(target => target.AssignedTo, options => options.MapFrom(source => source.Owner!.DisplayName))
                .ForMember(target => target.NoOfPreviousSubmissions, options => options.Ignore())
                .ForMember(d => d.Qa1CompletedBy, o => o.Ignore());
            
            CreateMap<EnrolmentQa2QueueEntry, EnrolmentQueueEntryDto>()
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
                    source => source.SupportWorker!.DisplayName
                ))
                .ForMember(target => target.Notes, options => options.MapFrom(source => source.Notes))
                .ForMember(target => target.AssignedTo, options => options.MapFrom(source => source.Owner!.DisplayName))
                .ForMember(target => target.NoOfPreviousSubmissions, options => options.Ignore())
                .ForMember(d => d.Qa1CompletedBy, o => o.Ignore());

            CreateMap<EnrolmentEscalationQueueEntry, EnrolmentQueueEntryDto>()
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
                    source => source.SupportWorker!.DisplayName
                ))
                .ForMember(target => target.Notes, options => options.MapFrom(source => source.Notes))
                .ForMember(target => target.AssignedTo, options => options.MapFrom(source => source.Owner!.DisplayName))
                .ForMember(target => target.NoOfPreviousSubmissions, options => options.Ignore())
                .ForMember(d => d.Qa1CompletedBy, o => o.Ignore());
        }
    }
}