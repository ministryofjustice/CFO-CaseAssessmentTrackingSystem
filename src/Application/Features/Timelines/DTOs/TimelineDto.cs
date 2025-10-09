using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Timelines.DTOs;

public class TimelineDto
{
    public string Title { get; set; } = default!;
    public string User { get; set; } = default!;
    public string Line1 { get; set; } = default!;
    public string? Line2 { get; set; } 
    public string? Line3 { get; set; } 
    public DateTime OccurredOn { get; set; }
    public string UserTenantId { get; set; } = default!;

    private class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Timeline, TimelineDto>(MemberList.None)
                .ForMember(t => t.Title, options => {
                    options.MapFrom(source => source.EventType.Name);
                })
                .ForMember(t => t.User, options => {
                    options.MapFrom(source =>
                                    source.CreatedByUser != null
                                         ? source.CreatedByUser.DisplayName
                                         : "System Update");
                })
                .ForMember(t => t.Line1, options => {
                    options.MapFrom(source => source.Line1);
                }).ForMember(t => t.Line2, options => {
                    options.MapFrom(source => source.Line2);
                }).ForMember(t => t.Line3, options => {
                    options.MapFrom(source => source.Line3);
                }).ForMember(t => t.OccurredOn, options => {
                    options.MapFrom(source => source.Created);
                }).ForMember(t => t.UserTenantId, options => {
                    options.MapFrom(source =>
                                    source.CreatedByUser != null
                                        ? source.CreatedByUser.TenantId
                                        : null);
                });
        }
    }
}