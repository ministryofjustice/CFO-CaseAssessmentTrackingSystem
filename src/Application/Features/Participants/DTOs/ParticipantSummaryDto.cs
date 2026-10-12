using Cfo.Cats.Application.Features.PathwayPlans.DTOs;
using Cfo.Cats.Domain.Entities.Assessments;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.DTOs;

/// <summary>
/// Represents the initial dashboard 
/// </summary>
public class ParticipantSummaryDto
{
    public required string Id { get; init; }

    /// <summary>
    /// The full name of the participant
    /// </summary>
    public required string ParticipantName { get; init; }

    /// <summary>
    /// The current location of the participant
    /// </summary>
    public required string Location { get; init; }

    public required LocationType LocationType { get; init; }

    public string? EnrolmentLocation { get; init; }

    /// <summary>
    /// The participant's date of birth
    /// </summary>
    public required DateOnly DateOfBirth { get; init; }
    public DateTime? RiskDue { get; init; }
    public int? RiskDueInDays => (RiskDue - DateTime.UtcNow.Date)?.Days;
    public string? Nationality { get; init; }

    public string? EnrolmentLocationJustification { get; init; }

    /// <summary>
    /// The assessment justification provided by the participant owner
    /// </summary>
    public string? AssessmentJustification { get; init; }

    /// <summary>
    ///  The current enrolment status of the participant
    /// </summary>
    public EnrolmentStatus EnrolmentStatus { get; init; } = EnrolmentStatus.IdentifiedStatus;

    /// <summary>
    ///  The current enrolment status of the participant
    /// </summary>
    public ConsentStatus ConsentStatus { get; init; } = ConsentStatus.PendingStatus;

    public DateOnly? DateOfFirstConsent { get; init; }

    /// <summary>
    /// The person who "owns" this participant's case. Usually the support worker.
    /// </summary>
    public string? OwnerName { get; set; }

    /// <summary>
    /// The id of the user who "owns" this participant's case.
    /// </summary>
    public string? OwnerId { get; init; }

    /// <summary>
    /// The Tenant who "owns" this participant's case. 
    /// </summary>
    public string? TenantName { get; init; }

    /// <summary>
    /// The TenantId of the owner. Used for archive access gating.
    /// </summary>
    public string? OwnerTenantId { get; set; }

    public AssessmentSummaryDto[] Assessments { get; init; } = [];

    public RiskSummaryDto? LatestRisk { get; init; }

    public BioSummaryDto? BioSummary { get; init; }

    public PathwayPlanSummaryDto? PathwayPlan { get; init; }

    public PriSummaryDto? LatestPri { get; init; }

    public bool HasActiveRightToWork { get; init; }
    public bool IsRightToWorkRequired { get; set; }

    public DateTime LastSync { get; init; }

    public DateTime? BioDue { get; init; }
    public int? BioDueInDays => (BioDue - DateTime.UtcNow.Date)?.Days;

    public bool IsActive { get; init; }
    public DateOnly? DeactivatedInFeed { get; init; }
    public DateOnly? PostLicenceCaseClosureEnd => DeactivatedInFeed?.AddDays(30);

    /// <summary>
    ///  The risk due reason of the participant
    /// </summary>
    public RiskDueReason RiskDueReason { get; init; } = RiskDueReason.NewEntry;

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
                .ForMember(target => target.OwnerId, options => options.MapFrom(source => source.OwnerId))
                .ForMember(target => target.TenantName, options => options.MapFrom(source => source.Owner!.TenantName))
                .ForMember(target => target.OwnerTenantId, options => options.MapFrom(source => source.Owner!.TenantId))
                .ForMember(target => target.ParticipantName, options => options.MapFrom(source => source.FirstName + ' ' + source.LastName))
                .ForMember(dest => dest.RiskDue, opt => opt.MapFrom(src => src.RiskDue))
                .ForMember(dest => dest.RiskDueInDays, opt => opt.MapFrom(src => src.RiskDueInDays()))
                .ForMember(dest => dest.Nationality, opt => opt.MapFrom(src => src.Nationality))
                .ForMember(dest => dest.EnrolmentLocationJustification, opt => opt.MapFrom(src => src.EnrolmentLocationJustification))
                .ForMember(dest => dest.AssessmentJustification, opt => opt.MapFrom(src => src.AssessmentJustification))
                .ForMember(dest => dest.LastSync, options => options.MapFrom(src => src.LastSyncDate ?? src.Created))
                .ForMember(dest => dest.BioDue, opt => opt.MapFrom(src => src.BioDue))
                .ForMember(dest => dest.BioDueInDays, opt => opt.MapFrom(src => src.BioDueInDays()))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive()))
                .ForMember(dest => dest.DeactivatedInFeed, opt => opt.MapFrom(src => src.DeactivatedInFeed));

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
    public Guid? AssessmentId { get; init; }

    /// <summary>
    /// If there are any assessments, these are the dates the latest one was created.
    /// </summary>
    public DateTime? AssessmentDate { get; init; }

    /// <summary>
    /// Who created the most recent assessment (if available)
    /// </summary>
    public string? AssessmentCreator { get; init; }

    /// <summary>
    /// Has the latest assessment been scored? This can be a surrogate for
    /// submitted and should make the assessment read-only
    /// </summary>
    public bool? AssessmentScored { get; init; }

    /// <summary>
    /// Date the latest assessment has been completed
    /// </summary>
    public DateTime? Completed { get; init; }
}

public class BioSummaryDto
{
    /// <summary>
    /// The id of the one and only Bio, at least for now
    /// </summary>
    public Guid? BioId { get; init; }

    /// <summary>
    /// The date when Bio was created.
    /// </summary>
    public DateTime? BioDate { get; init; }

    /// <summary>
    /// Who created the Bio (if available)
    /// </summary>
    public string? BioCreator { get; init; }

    /// <summary>
    /// Status of the Bio
    /// </summary>
    public BioStatus BioStatus { get; init; } = BioStatus.NotStarted;
}
