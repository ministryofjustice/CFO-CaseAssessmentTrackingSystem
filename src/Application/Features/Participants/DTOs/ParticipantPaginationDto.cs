using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.DTOs;

[Description("Participants")]
public class ParticipantPaginationDto
{
    [Description("Participant Id")]
    public string Id { get; set; } = default!;
    
    [Description("Status")]
    public EnrolmentStatus EnrolmentStatus { get; set; } = default!;
    
    [Description("Consent")]
    public ConsentStatus ConsentStatus { get; set; } = default!;
    
    [Description("Participant")]
    public string ParticipantName { get; set; } = default!;
    
    [Description("Location")]
    public LocationDto CurrentLocation { get; set; } = default!;
    
    [Description("Enrolled At")]
    public LocationDto? EnrolmentLocation { get; set; }
    
    [Description("Assignee")]
    public string Owner { get; set; } = default!;
    
    [Description("Tenant")]
    public string Tenant { get; set; } = default!;

    [Description("Risk Due")]
    public DateTime? RiskDue { get; set; }

    [Description] 
    public RiskDueReason RiskDueReason { get; set; } = RiskDueReason.Unknown;

    private class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Participant, ParticipantPaginationDto>(MemberList.None)
                .ForMember(target => target.Id, options => options.MapFrom(source => source.Id))
                .ForMember(target => target.EnrolmentStatus, options => options.MapFrom(source => source.EnrolmentStatus))
                .ForMember(target => target.ConsentStatus, options => options.MapFrom(source => source.ConsentStatus))
                .ForMember(target => target.ParticipantName, options => options.MapFrom(source => source.FirstName + " " + source.LastName))
                .ForMember(target => target.CurrentLocation, options => options.MapFrom(source => source.CurrentLocation))
                .ForMember(target => target.EnrolmentLocation, options => options.MapFrom(source => source.EnrolmentLocation))
                .ForMember(target => target.Owner, options => options.MapFrom(source => source.Owner!.DisplayName))
                .ForMember(target => target.Tenant, options => options.MapFrom(source => source.Owner!.TenantName))
                .ForMember(target => target.RiskDue, options => options.MapFrom(source => source.RiskDue))
                .ForMember(target => target.RiskDueReason, options => options.MapFrom(source => source.RiskDueReason));
            
        }
    }

}