using System.ComponentModel.DataAnnotations;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.DTOs;

[Description("Unassigned Cases")]
public class UnassignedCaseDto
{
    [Description("Participant Id")]
    public string Id { get; set; } = default!;
    
    [Description("Status")]
    public EnrolmentStatus EnrolmentStatus { get; set; } = default!;
    
    [Description("Consent")]
    public ConsentStatus ConsentStatus { get; set; } = default!;

    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;
    
    [Description("Participant")]
    public string ParticipantName => $"{FirstName} {LastName}";
    
    [Description("Location")]
    public LocationDto CurrentLocation { get; set; } = default!;
    
    [Description("Enrolled At")]
    public LocationDto? EnrolmentLocation { get; set; }
    
    [Description("Tenant")]
    public string TenantId { get; set; } = default!;

    [Description("Tenant Name")]
    public string TenantName { get; set; } = default!;

    [Description("Last Modified")]
    public DateTime? LastModified { get; set; }

    [Description("Has Incoming Transfer")]
    public bool HasIncomingTransfer { get; set; }

    [Description("Incoming Transfer Id")]
    public Guid? IncomingTransferId { get; set; }

    private class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Participant, UnassignedCaseDto>()
                .ForMember(dest => dest.ParticipantName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.TenantName, opt => opt.Ignore())
                .ForMember(dest => dest.HasIncomingTransfer, opt => opt.Ignore())
                .ForMember(dest => dest.IncomingTransferId, opt => opt.Ignore());
        }
    }
}
