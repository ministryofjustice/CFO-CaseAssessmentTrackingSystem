namespace Cfo.Cats.Application.Features.HmppsExternalApi.DTOs;

/// <summary>
/// Tier calculation details for a person, as returned by the HMPPS External API's
/// GET /v1/persons/{hmppsId}/tier/details endpoint.
/// </summary>
public class TierDetailsDto
{
    /// <summary>
    /// The calculated tier (e.g. "A" to "G").
    /// </summary>
    public string? TierScore { get; init; }
    public Guid? CalculationId { get; init; }
    public DateTime? CalculationDate { get; init; }
    public bool? Provisional { get; init; }
    public TierCalculationDto? Data { get; init; }
}

public class TierCalculationDto
{
    public string? Tier { get; init; }
    public bool? Provisional { get; init; }
    public string? CalculationVersion { get; init; }
    public TierDeliusInputsDto? DeliusInputs { get; init; }
    public TierAssessmentSummaryDto? AssessmentSummary { get; init; }
    public TierOasysInputsDto? OasysInputs { get; init; }
}

public class TierDeliusInputsDto
{
    public bool? IsFemale { get; init; }
    public decimal? RsrScore { get; init; }
    public decimal? OgrsScore { get; init; }
    public bool? HasNoMandate { get; init; }
    public TierRegistrationsDto? Registrations { get; init; }
    public bool? PreviousEnforcementActivity { get; init; }
    public DateOnly? LatestReleaseDate { get; init; }
    public bool? HasActiveEvent { get; init; }
    public DateOnly? LatestSentencingAct2026ExcludedOffenceDate { get; init; }
    public DateOnly? LatestChildSexualExploitationSentenceDate { get; init; }
}

public class TierRegistrationsDto
{
    public bool? HasIomNominal { get; init; }
    public bool? HasLiferIpp { get; init; }
    public bool? HasDomesticAbuse { get; init; }
    public bool? HasStalking { get; init; }
    public bool? HasChildProtection { get; init; }

    /// <summary>
    /// Complexity factor codes, e.g. "TERRORISM", "RISK_TO_CHILDREN".
    /// </summary>
    public List<string>? ComplexityFactors { get; init; }

    /// <summary>
    /// Risk of Serious Harm level, e.g. "VERY_HIGH", "HIGH", "MEDIUM", "LOW".
    /// </summary>
    public string? Rosh { get; init; }
    public string? MappaLevel { get; init; }
    public string? MappaCategory { get; init; }
    public bool? Unsupervised { get; init; }
}

public class TierAssessmentSummaryDto
{
    public TierAssessmentDto? Assessment { get; init; }
    public TierAssessmentSectionDto? Accommodation { get; init; }
    public TierAssessmentSectionDto? EducationTrainingEmployability { get; init; }
    public TierAssessmentSectionDto? Relationships { get; init; }
    public TierAssessmentSectionDto? LifestyleAndAssociates { get; init; }
    public TierAssessmentSectionDto? DrugMisuse { get; init; }
    public TierAssessmentSectionDto? AlcoholMisuse { get; init; }
    public TierAssessmentSectionDto? ThinkingAndBehaviour { get; init; }
    public TierAssessmentSectionDto? Attitudes { get; init; }
}

public class TierAssessmentDto
{
    public long? Id { get; init; }
    public DateTime? CompletedDate { get; init; }
    public string? Type { get; init; }
    public string? Status { get; init; }
    public bool? SanIndicator { get; init; }
}

/// <summary>
/// An OASys criminogenic needs section. Linked fields are free-text ("Yes", "No"); question answers are keyed by question code.
/// </summary>
public class TierAssessmentSectionDto
{
    public string? Section { get; init; }
    public string? LinkedToReOffending { get; init; }
    public string? LinkedToHarm { get; init; }
    public Dictionary<string, string?>? QuestionAnswers { get; init; }
    public decimal? Score { get; init; }
    public string? ParentalResponsibilities { get; init; }
    public string? Impulsivity { get; init; }
    public string? TemperControl { get; init; }
}

public class TierOasysInputsDto
{
    public TierPredictorsDto? Predictors { get; init; }
    public bool? EverCommittedSexualOffence { get; init; }
}

public class TierPredictorsDto
{
    public DateTime? CompletedDate { get; init; }
    public string? AssessmentType { get; init; }
    public string? OutputVersion { get; init; }
    public TierPredictorOutputDto? Output { get; init; }
}

public class TierPredictorOutputDto
{
    public TierPredictorDto? AllReoffendingPredictor { get; init; }
    public TierPredictorDto? ViolentReoffendingPredictor { get; init; }
    public TierPredictorDto? SeriousViolentReoffendingPredictor { get; init; }
    public TierPredictorDto? DirectContactSexualReoffendingPredictor { get; init; }
    public TierPredictorDto? IndirectImageContactSexualReoffendingPredictor { get; init; }
    public TierPredictorDto? CombinedSeriousReoffendingPredictor { get; init; }
}

public class TierPredictorDto
{
    public string? AlgorithmVersion { get; init; }

    /// <summary>
    /// "STATIC" or "DYNAMIC".
    /// </summary>
    public string? StaticOrDynamic { get; init; }
    public decimal? Score { get; init; }

    /// <summary>
    /// e.g. "LOW", "MEDIUM", "HIGH", "VERY_HIGH", "NOT_APPLICABLE".
    /// </summary>
    public string? Band { get; init; }
}
