using Cfo.Cats.Domain.Entities.Activities;
using Microsoft.Extensions.Options;

namespace Cfo.Cats.Application.Features.Activities.DTOs
{
    public class ActivityQueueEntryDto
    {
        public Guid Id { get; set; }

        public Guid ActivityId { get; set; } = default!;
        public DateTime Created { get; set; } = default!;
        public string TenantId { get; set; } = default!;
        public string TenantName { get; set; } = default!;
        public string ParticipantName { get; set; } = default!;        
        public string? ParticipantId { get; set; }
        
        public string SupportWorker { get; set; } = default!;

        public string? AssignedTo { get; set; }

        public bool IsCompleted { get; set; }
        public bool IsAccepted { get; set; }

        public ActivitySummaryDto Activity { get; set; } = default!;

        public DateTime CommencedOn => Activity.CommencedOn;

        public DateTime Expiry => Activity.CommencedOn.AddMonths(3);

        public NoteDto[] Notes { get; set; } = [];

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
                    .ForMember(target => target.Activity, options => options.MapFrom(source => source.Activity));

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
                     .ForMember(target => target.SupportWorker, options => options.MapFrom(
                     source => source.Participant!.Owner!.DisplayName
                     ))
                     .ForMember(target => target.Notes, options => options.MapFrom(source => source.Notes))
                      .ForMember(target => target.AssignedTo, options => options.MapFrom(source => source.Owner!.DisplayName))
                    .ForMember(target => target.Activity, options => options.MapFrom(source => source.Activity));

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
                    .ForMember(target => target.Activity, options => options.MapFrom(source => source.Activity));

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
                    .ForMember(target => target.Activity, options => options.MapFrom(source => source.Activity));
            }
        }
    }
}