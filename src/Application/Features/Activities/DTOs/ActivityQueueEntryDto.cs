using Cfo.Cats.Domain.Entities.Activities;

namespace Cfo.Cats.Application.Features.Activities.DTOs;

public class ActivityQueueEntryDto
{
    public Guid Id { get; init; }

    public Guid ActivityId { get; init; } = Guid.Empty!;
    public DateTime Created { get; init; }
    public string TenantId { get; init; } = null!;
    public string TenantName { get; init; } = null!;
    public string ParticipantName { get; init; } = null!;        
    public string? ParticipantId { get; init; }
        
    public string SupportWorker { get; init; } = null!;

    public string? AssignedTo { get; init; }

    public bool IsCompleted { get; init; }
    public bool IsAccepted { get; init; }

    public ActivitySummaryDto Activity { get; init; } = null!;

    public DateTime CommencedOn => Activity.CommencedOn;

    public DateTime Expiry => Activity.Expiry;

    public int NoOfPreviousSubmissions { get; set; }

    public NoteDto[] Notes { get; init; } = [];

    public string? Qa1CompletedBy { get; set; }
    
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ActivityPqaQueueEntry, ActivityQueueEntryDto>()
                .ForMember(target => target.ActivityId,
                options => options.MapFrom(source => source.ActivityId))
                .ForMember(target => target.Created,
                options => options.MapFrom(source => source.Created))
                .ForMember(target => target.TenantId,
                options => options.MapFrom(source => source.TenantId))
                .ForMember(target => target.TenantName,
                options => options.MapFrom(source => source.Tenant!.Name))
                .ForMember(target => target.ParticipantName, options =>
                {
                    options.MapFrom(target => target.Participant!.FirstName + " " + target.Participant.LastName);
                })
                .ForMember(target => target.ParticipantId,
                options => options.MapFrom(source => source.ParticipantId))
                .ForMember(target => target.SupportWorker, options => options.MapFrom(
                source => source.Participant!.Owner!.DisplayName
                ))
                .ForMember(target => target.Notes, options => options.MapFrom(source => source.Notes))
                .ForMember(target => target.IsCompleted, options => options.MapFrom(source => source.IsCompleted))
                .ForMember(target => target.IsAccepted, options => options.MapFrom(source => source.IsAccepted))
                .ForMember(target => target.AssignedTo, options => options.MapFrom(source => source.Owner!.DisplayName))
                .ForMember(target => target.Activity, options => options.MapFrom(source => source.Activity))
                .ForMember(target => target.NoOfPreviousSubmissions, options => options.Ignore());

            CreateMap<ActivityQa1QueueEntry, ActivityQueueEntryDto>()
                .ForMember(target => target.ActivityId,
                    options => options.MapFrom(source => source.ActivityId))
                .ForMember(target => target.Created,
                    options => options.MapFrom(source => source.Created))
                .ForMember(target => target.TenantId,
                    options => options.MapFrom(source => source.TenantId))
                .ForMember(target => target.TenantName,
                    options => options.MapFrom(source => source.Tenant!.Name))
                .ForMember(target => target.ParticipantName, options =>
                {
                    options.MapFrom(target => target.Participant!.FirstName + " " + target.Participant.LastName);
                })
                .ForMember(target => target.ParticipantId,
                    options => options.MapFrom(source => source.ParticipantId))
                .ForMember(target => target.SupportWorker, options =>
                    options.MapFrom(source => source.Participant!.Owner!.DisplayName
                    ))
                .ForMember(target => target.Notes, options => options.MapFrom(source => source.Notes))
                .ForMember(target => target.AssignedTo, options => options.MapFrom(source => source.Owner!.DisplayName))
                .ForMember(target => target.Activity, options => options.MapFrom(source => source.Activity))
                .ForMember(target => target.NoOfPreviousSubmissions, options => options.Ignore());

            CreateMap<ActivityQa2QueueEntry, ActivityQueueEntryDto>()
                .ForMember(target => target.ActivityId,
                options => options.MapFrom(source => source.ActivityId))
                .ForMember(target => target.Created,
                options => options.MapFrom(source => source.Created))
                .ForMember(target => target.TenantId,
                options => options.MapFrom(source => source.TenantId))
                .ForMember(target => target.TenantName,
                options => options.MapFrom(source => source.Tenant!.Name))
                .ForMember(target => target.ParticipantName, options =>
                {
                    options.MapFrom(target => target.Participant!.FirstName + " " + target.Participant.LastName);
                })
                .ForMember(target => target.ParticipantId,
                options => options.MapFrom(source => source.ParticipantId))
                .ForMember(target => target.SupportWorker, options => options.MapFrom(
                source => source.Participant!.Owner!.DisplayName
                ))
                .ForMember(target => target.Notes, options => options.MapFrom(source => source.Notes))
                .ForMember(target => target.AssignedTo, options => options.MapFrom(source => source.Owner!.DisplayName))
                .ForMember(target => target.Activity, options => options.MapFrom(source => source.Activity))
                .ForMember(target => target.NoOfPreviousSubmissions, options => options.Ignore())
                .ForMember(d => d.Qa1CompletedBy, o => o.Ignore());

            CreateMap<ActivityEscalationQueueEntry, ActivityQueueEntryDto>()
                .ForMember(target => target.ActivityId,
                options => options.MapFrom(source => source.ActivityId))
                .ForMember(target => target.Created,
                options => options.MapFrom(source => source.Created))
                .ForMember(target => target.TenantId,
                options => options.MapFrom(source => source.TenantId))
                .ForMember(target => target.TenantName,
                options => options.MapFrom(source => source.Tenant!.Name))
                .ForMember(target => target.ParticipantName, options =>
                {
                    options.MapFrom(target => target.Participant!.FirstName + " " + target.Participant.LastName);
                })
                .ForMember(target => target.ParticipantId,
                options => options.MapFrom(source => source.ParticipantId))
                .ForMember(target => target.SupportWorker, options => options.MapFrom(
                source => source.Participant!.Owner!.DisplayName
                ))
                .ForMember(target => target.Notes, options => options.MapFrom(source => source.Notes))
                .ForMember(target => target.AssignedTo, options => options.MapFrom(source => source.Owner!.DisplayName))
                .ForMember(target => target.Activity, options => options.MapFrom(source => source.Activity))
                .ForMember(target => target.NoOfPreviousSubmissions, options => options.Ignore())
                .ForMember(d => d.Qa1CompletedBy, o => o.Ignore());
        }
    }
}