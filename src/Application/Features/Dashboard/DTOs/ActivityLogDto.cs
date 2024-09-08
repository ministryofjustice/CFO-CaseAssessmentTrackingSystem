using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Dashboard.DTOs;

public class ActivityLogDto
{
    public string ParticipantId { get; set; } = default!;
    public string ParticipantName { get;set; } = default!;

    public TimelineEventType EventType { get;set; } = default!;

    public string Line1 { get;set; } = default!;
    public string? Line2 {get;set;}
    public string? Line3 {get;set;}
    public string CreatedBy {get;set;} = default!;
    public DateTime Created { get; set;}

    private class Mapper : Profile
    {
        public Mapper()
        {
            #nullable disable
            CreateMap<Timeline, ActivityLogDto>(MemberList.None)
                .ForMember(t => t.ParticipantId, o => o.MapFrom(s => s.ParticipantId))
                .ForMember(t => t.ParticipantName, o => o.MapFrom(s => s.Participant.FirstName + " " + s.Participant.LastName))
                .ForMember(t => t.EventType, o => o.MapFrom(s => s.EventType))
                .ForMember(t => t.Line1, o => o.MapFrom(s => s.Line1))
                .ForMember(t => t.Line2, o => o.MapFrom(s => s.Line2))
                .ForMember(t => t.Line3, o => o.MapFrom(s => s.Line3))
                .ForMember(t => t.CreatedBy, o => o.MapFrom(s => s.CreatedByUser.DisplayName))
                .ForMember(t => t.Created, o => o.MapFrom(s => s.Created));
        
        }
    }


}