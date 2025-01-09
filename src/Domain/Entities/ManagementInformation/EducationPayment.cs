using Cfo.Cats.Domain.Exceptions;

namespace Cfo.Cats.Domain.Entities.ManagementInformation;

public class EducationPayment
{
#pragma warning disable CS8618
    internal EducationPayment()
    {
    }
#pragma warning restore CS8618

    public Guid Id { get; set; } = Guid.CreateVersion7();
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public Guid ActivityId { get; set; }
    public DateTime ActivityApproved { get; set; }
    public string ParticipantId { get; set; }
    public string ContractId { get; set; }
    public int LocationId { get; set; }
    public string LocationType { get; set; }
    public string TenantId { get; set; }
    public bool EligibleForPayment { get; set; }
    public string? IneligibilityReason { get; set; }
    public string? CourseTitle { get; set; }
    public string? CourseLevel { get; set; }
}

public class EducationPaymentBuilder
{
    private Guid? _activityId;
    private string? _participantId;
    private string? _contractId;
    private DateTime? _activityApproved;
    private int? _locationId;
    private string? _locationType;
    private string? _tenantId;
    private bool? _eligibleForPayment;
    private string? _ineligibilityReason;
    private string? _courseTitle;
    private string? _courseLevel;

    public EducationPaymentBuilder WithActivity(Guid activityId)
    {
        _activityId = activityId;
        return this;
    }

    public EducationPaymentBuilder WithParticipantId(string participantId)
    {
        _participantId = participantId;
        return this;
    }

    public EducationPaymentBuilder WithContractId(string contractId)
    {
        _contractId = contractId;
        return this;
    }

    public EducationPaymentBuilder WithApproved(DateTime approved)
    {
        _activityApproved = approved;
        return this;
    }

    public EducationPaymentBuilder WithLocationId(int locationId)
    {
        _locationId = locationId;
        return this;
    }

    public EducationPaymentBuilder WithLocationType(string locationType)
    {
        _locationType = locationType;
        return this;
    }

    public EducationPaymentBuilder WithTenantId(string tenantId)
    {
        _tenantId = tenantId;
        return this;
    }

    public EducationPaymentBuilder WithEligibleForPayment(bool eligibleForPayment)
    {
        _eligibleForPayment = eligibleForPayment;
        return this;
    }

    public EducationPaymentBuilder WithIneligibilityReason(string? ineligibilityReason)
    {
        _ineligibilityReason = ineligibilityReason;
        return this;
    }

    public EducationPaymentBuilder WithCourseTitle(string? courseTitle)
    {
        _courseTitle = courseTitle;
        return this;
    }

    public EducationPaymentBuilder WithCourseLevel(string? courseLevel)
    {
        _courseLevel = courseLevel;
        return this;
    }

    public EducationPayment Build()
    {
        var payment = new EducationPayment()
        {
            ActivityId = _activityId ?? throw new InvalidBuilderException("ActivityId"),
            ActivityApproved = _activityApproved ?? throw new InvalidBuilderException("ActivityApproved"),
            ContractId = _contractId ?? throw new InvalidBuilderException("ContractId"),
            EligibleForPayment = _eligibleForPayment ?? throw new InvalidBuilderException("EligibleForPayment"),
            IneligibilityReason = _eligibleForPayment == false && _ineligibilityReason == null
                                        ? throw new InvalidBuilderException("IneligibilityReason")
                    : _ineligibilityReason,
            LocationId = _locationId ?? throw new InvalidBuilderException("LocationType"),
            LocationType = _locationType ?? throw new InvalidBuilderException("LocationType"),
            ParticipantId = _participantId ?? throw new InvalidBuilderException("ParticipantId"),
            TenantId = _tenantId ?? throw new InvalidBuilderException("TenantId"),
            CourseTitle = _courseTitle ?? throw new InvalidBuilderException("CourseTitle"),
            CourseLevel = _courseLevel ?? throw new InvalidBuilderException("CourseLevel"),
        };
        return payment;
    }
}