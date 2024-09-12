using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.DTOs;

[Description("Participants")]
public class ParticipantDto
{
    [Description("CATS Identifier")]
    public required string Id { get; set; }
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public DateTime? RiskDue { get; set; }
    public int? RiskDueInDays { get; set; }

    [Description("Enrolment Status")]
    public EnrolmentStatus? EnrolmentStatus { get; set; }

    [Description("Consent Status")]
    public ConsentStatus? ConsentStatus { get;set; }

    [Description("Current Location")]
    public LocationDto CurrentLocation { get; set; } = default!;
    
    [Description("Enrolment Location")]
    public LocationDto? EnrolmentLocation { get; set; }
    
    [Description("Enrolment Justification Reason")]
    public string? EnrolmentLocationJustification { get; set; }

    [Description("Assessment Justification Reason")]
    public string? AssessmentJustification { get; set; }

    public ConsentDto[] Consents { get; set; } = [];

    public RightToWorkDto[] RightToWorks { get; set; } = [];

    public ParticipantNoteDto[] Notes { get; set; } = [];

    public ExternalIdentifierDto[] ExternalIdentifiers {get;set;} = [];

    public string TenantId { get; set; } = default!;

    public string SupportWorker { get;set; } = default!;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Participant, ParticipantDto>()
                .ForMember(target => target.CurrentLocation,
                    options => options.MapFrom(source => source.CurrentLocation))
                .ForMember(target => target.Consents,
                    options => options.MapFrom(source => source.Consents.ToArray()))
                .ForMember(target => target.RightToWorks,
                    options => options.MapFrom(source => source.RightToWorks.ToArray()))
                .ForMember(target => target.Notes,
                    options => options.MapFrom(source => source.Notes.ToArray()))
                .ForMember(target => target.TenantId, options => options.MapFrom(s => s.Owner!.TenantId))
                .ForMember(target => target.ExternalIdentifiers, options => options.MapFrom(s => s.ExternalIdentifiers.ToArray()))
#nullable disable
                .ForMember(target => target.SupportWorker, options => options.MapFrom(source => source.Owner.DisplayName))
                .ForMember(dest => dest.RiskDue, opt => opt.MapFrom(src => src.RiskDue))
                .ForMember(dest => dest.RiskDueInDays, opt => opt.MapFrom(src => src.RiskDueInDays()));
        }
    }
}
