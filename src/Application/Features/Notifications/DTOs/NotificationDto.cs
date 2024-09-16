using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Domain.Entities.Notifications;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace Cfo.Cats.Application.Features.Notifications.DTOs;

public class NotificationDto
{
    public string Heading { get; set; } = default!;
    public string Details { get; set; } = default!;
    
    public DateTime NotificationDate { get; set; }

    public string? Link { get;set; }

    private class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Notification, NotificationDto>(MemberList.Destination)
                .ForMember(t => t.Heading, opt => opt.MapFrom(src => src.Heading))
                .ForMember(t => t.Details, opt => opt.MapFrom(src => src.Details))
                .ForMember(t => t.NotificationDate, opt => opt.MapFrom(src => src.Created))
                .ForMember(t => t.Link, opt => opt.MapFrom(src => src.Link));
        }
    }

}