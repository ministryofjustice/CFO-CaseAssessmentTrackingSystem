namespace Cfo.Cats.Application.Features.HmppsExternalApi.DTOs;

/// <summary>
/// Key dates for a person's latest prison sentence, as returned by the HMPPS External API's
/// GET /v1/persons/{hmppsId}/sentences/latest-key-dates-and-adjustments endpoint. Sourced from NOMIS only.
/// </summary>
/// <remarks>Sentence adjustments are also returned by the endpoint but are not mapped.</remarks>
public class SentenceKeyDatesDto
{
    public SentenceKeyDateDto? AutomaticRelease { get; init; }
    public SentenceKeyDateDto? ConditionalRelease { get; init; }
    public SentenceKeyDateDto? DtoPostRecallRelease { get; init; }
    public SentenceKeyDateDto? EarlyTerm { get; init; }
    public HomeDetentionCurfewDateDto? HomeDetentionCurfew { get; init; }
    public SentenceKeyDateDto? LateTerm { get; init; }
    public SentenceKeyDateDto? LicenceExpiry { get; init; }
    public SentenceKeyDateDto? MidTerm { get; init; }
    public SentenceKeyDateDto? NonParole { get; init; }
    public SentenceKeyDateDto? ParoleEligibility { get; init; }
    public SentenceKeyDateDto? PostRecallRelease { get; init; }
    public ReleaseDateDto? Release { get; init; }
    public SentenceDateDto? Sentence { get; init; }
    public TopupSupervisionDto? TopupSupervision { get; init; }
    public DateOnly? ActualParoleDate { get; init; }
    public DateOnly? EarlyRemovalSchemeEligibilityDate { get; init; }
    public DateOnly? ReleaseOnTemporaryLicenceDate { get; init; }
    public DateOnly? TariffDate { get; init; }
}

public class SentenceKeyDateDto
{
    public DateOnly? Date { get; init; }
    public DateOnly? OverrideDate { get; init; }
    public DateOnly? CalculatedDate { get; init; }

    /// <summary>
    /// The date in force: a manual override takes precedence over the recorded date.
    /// </summary>
    public DateOnly? EffectiveDate => OverrideDate ?? Date;
}

public class HomeDetentionCurfewDateDto
{
    public DateOnly? ActualDate { get; init; }
    public DateOnly? EligibilityDate { get; init; }
    public DateOnly? EligibilityCalculatedDate { get; init; }
    public DateOnly? EligibilityOverrideDate { get; init; }
    public DateOnly? EndDate { get; init; }
}

public class ReleaseDateDto
{
    public DateOnly? Date { get; init; }
    public DateOnly? ConfirmedDate { get; init; }
}

public class SentenceDateDto
{
    public DateOnly? StartDate { get; init; }
    public DateOnly? EffectiveEndDate { get; init; }
    public DateOnly? ExpiryDate { get; init; }
    public DateOnly? ExpiryCalculatedDate { get; init; }
    public DateOnly? ExpiryOverrideDate { get; init; }
}

public class TopupSupervisionDto
{
    public DateOnly? StartDate { get; init; }
    public DateOnly? ExpiryDate { get; init; }
    public DateOnly? ExpiryCalculatedDate { get; init; }
    public DateOnly? ExpiryOverrideDate { get; init; }
}
