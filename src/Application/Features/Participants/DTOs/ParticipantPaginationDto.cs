using System.ComponentModel.DataAnnotations;
using Cfo.Cats.Application.Features.Labels.DTOs;
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

    [Display(Name = "Labels", Description = "A collection of labels linked to this participant")]
    public LabelDto[] Labels { get; set; } = [];
}