using Cfo.Cats.Application.Features.PathwayPlans.DTOs;
using Cfo.Cats.Domain.Entities.Assessments;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.DTOs;

/// <summary>
/// Represents the initial dashboard 
/// </summary>
public class ParticipantSummaryDto
{
    public required string Id { get; set; }

    /// <summary>
    /// The full name of the participant
    /// </summary>
    public required string ParticipantName { get; set; }

    /// <summary>
    /// The current location of the participant
    /// </summary>
    public required string Location { get; set; }

    public required LocationType LocationType { get; set; }

    public string? EnrolmentLocation { get; set; }

    /// <summary>
    /// The participant's date of birth
    /// </summary>
    public required DateOnly DateOfBirth { get; set; }
    public DateTime? RiskDue { get; set; }
    public int? RiskDueInDays { get; set; }
    public string? Nationality { get; set; }

    public string? EnrolmentLocationJustification { get; set; }

    /// <summary>
    ///  The current enrolment status of the participant
    /// </summary>
    public EnrolmentStatus EnrolmentStatus { get; set; } = EnrolmentStatus.IdentifiedStatus;

    /// <summary>
    ///  The current enrolment status of the participant
    /// </summary>
    public ConsentStatus ConsentStatus { get; set; } = ConsentStatus.PendingStatus;

    public DateOnly? DateOfFirstConsent { get; set; }

    /// <summary>
    /// The person who "owns" this participant's case. Usually the support worker.
    /// </summary>
    public required string OwnerName { get; set; }

    /// <summary>
    /// The Tenant who "owns" this participant's case. 
    /// </summary>
    public required string TenantName { get; set; }

    public AssessmentSummaryDto[] Assessments { get; set; } = [];

    public RiskSummaryDto? LatestRisk { get; set; }

    public BioSummaryDto? BioSummary { get; set; }

    public PathwayPlanSummaryDto? PathwayPlan { get; set; }

    public PriSummaryDto? LatestPri { get; set; }

    public bool HasActiveRightToWork { get; set; }
    public bool IsRightToWorkRequired { get; set; }

    public DateTime LastSync { get; set; }

    public DateTime? BioDue { get; set; }
    public int? BioDueInDays { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Participant, ParticipantSummaryDto>(MemberList.None)
                .ForMember(target => target.Id, options => options.MapFrom(source => source.Id))
                .ForMember(target => target.DateOfFirstConsent,
                    options => options.MapFrom(source => source.DateOfFirstConsent))
                .ForMember(target => target.Location, options => options.MapFrom(source => source.CurrentLocation.Name))
                .ForMember(target => target.LocationType, options => options.MapFrom(source => source.CurrentLocation.LocationType))
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                .ForMember(target => target.EnrolmentLocation, options => options.MapFrom(source => source.EnrolmentLocation.Name))
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                .ForMember(target => target.OwnerName, options => options.MapFrom(source => source.Owner!.DisplayName))
                .ForMember(target => target.TenantName, options => options.MapFrom(source => source.Owner!.TenantName))
                .ForMember(target => target.ParticipantName, options => options.MapFrom(source => source.FirstName + ' ' + source.LastName))
                .ForMember(dest => dest.RiskDue, opt => opt.MapFrom(src => src.RiskDue))
                .ForMember(dest => dest.RiskDueInDays, opt => opt.MapFrom(src => src.RiskDueInDays()))
                .ForMember(dest => dest.Nationality, opt => opt.MapFrom(src => src.Nationality))
                .ForMember(dest => dest.EnrolmentLocationJustification, opt => opt.MapFrom(src => src.EnrolmentLocationJustification))
                .ForMember(dest => dest.LastSync, options => options.MapFrom(src => src.LastSyncDate ?? src.Created))
                .ForMember(dest => dest.BioDue, opt => opt.MapFrom(src => src.BioDue))
                .ForMember(dest => dest.BioDueInDays, opt => opt.MapFrom(src => src.BioDueInDays()));

            CreateMap<ParticipantAssessment, AssessmentSummaryDto>()
                .ForMember(target => target.AssessmentId, options => options.MapFrom(source => source.Id))
                .ForMember(target => target.AssessmentDate, options => options.MapFrom(source => source.Created))
                .ForMember(target => target.AssessmentCreator, options => options.MapFrom(source => source.CreatedBy))
                .ForMember(target => target.AssessmentScored, options => options.MapFrom(source => source.Scores.All(s => s.Score >= 0)))
                .ForMember(target => target.Completed, options => options.MapFrom(source => source.Completed));

            CreateMap<Domain.Entities.Bios.ParticipantBio, BioSummaryDto>()
                .ForMember(target => target.BioId, options => options.MapFrom(source => source.Id))
                .ForMember(target => target.BioDate, options => options.MapFrom(source => source.Created))
                .ForMember(target => target.BioStatus, options => options.MapFrom(source => source.Status))
                .ForMember(target => target.BioCreator, options => options.MapFrom(source => source.CreatedBy));
        }
    }
}

public class AssessmentSummaryDto
{
    /// <summary>
    /// The id of the latest assessment
    /// </summary>
    public Guid? AssessmentId { get; set; }

    /// <summary>
    /// If there are any assessments these are the dates they latest one was created.
    /// </summary>
    public DateTime? AssessmentDate { get; set; }

    /// <summary>
    /// Who created the most recent assessment (if available)
    /// </summary>
    public string? AssessmentCreator { get; set; }

    /// <summary>
    /// Has the latest assessment been scored? This can be a surrogate for
    /// submitted and should make the assessment read-only
    /// </summary>
    public bool? AssessmentScored { get; set; }

    /// <summary>
    /// Date the latest assessment has been complted
    /// </summary>
    public DateTime? Completed { get; private set; }
}

public class BioSummaryDto
{
    /// <summary>
    /// The id of the one and only Bio, atleast for now
    /// </summary>
    public Guid? BioId { get; set; }

    /// <summary>
    /// The date when Bio was created.
    /// </summary>
    public DateTime? BioDate { get; set; }

    /// <summary>
    /// Who created the Bio (if available)
    /// </summary>
    public string? BioCreator { get; set; }

    /// <summary>
    /// Status of the Bio
    /// </summary>
    public BioStatus BioStatus { get; set; } = BioStatus.NotStarted;
}