﻿using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.QualityAssurance.DTOs;

public class EnrolmentQueueEntryDto
{
    public Guid Id { get; set; }
    public string ParticipantId { get; set; } = default!; 
    public DateTime Created { get; set; } = default!;
    public string TenantId { get; set; } = default!;
    public string TenantName { get; set; } = default!;
    public string ParticipantName { get; set; } = default!;

    public string SupportWorker { get; set; } = default!;

    public string? AssignedTo { get; set; } 

    public bool IsCompleted { get; set; }
    public bool IsAccepted { get; set; }
    
    public NoteDto[] Notes { get; set; } = [];

    public int NoOfPreviousSubmissions { get; set; }

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
                .ForMember(target => target.NoOfPreviousSubmissions, options => options.Ignore());

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
                .ForMember(target => target.NoOfPreviousSubmissions, options => options.Ignore());

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
                .ForMember(target => target.NoOfPreviousSubmissions, options => options.Ignore());

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
                .ForMember(target => target.NoOfPreviousSubmissions, options => options.Ignore());

        }
    }

}

