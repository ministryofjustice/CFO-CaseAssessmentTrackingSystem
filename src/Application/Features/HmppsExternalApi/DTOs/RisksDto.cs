namespace Cfo.Cats.Application.Features.HmppsExternalApi.DTOs;

/// <summary>
/// Risk of Serious Harm (ROSH) risks for a person, as returned by the HMPPS External API's
/// GET /v1/persons/{hmppsId}/risks/serious-harm endpoint.
/// </summary>
public class RisksDto
{
    public DateTime? AssessedOn { get; init; }
    public RiskToSelfDto? RiskToSelf { get; init; }
    public OtherRisksDto? OtherRisks { get; init; }
    public RiskSummaryDto? Summary { get; init; }
}

public class RiskToSelfDto
{
    public RiskDetailDto? Suicide { get; init; }
    public RiskDetailDto? SelfHarm { get; init; }
    public RiskDetailDto? Custody { get; init; }
    public RiskDetailDto? HostelSetting { get; init; }
    public RiskDetailDto? Vulnerability { get; init; }
}

/// <summary>
/// Presence and history of a specific risk type. Fields are free-text/enum ("YES", "NO", "DK", "NA") as returned by the HMPPS External API.
/// </summary>
public class RiskDetailDto
{
    public string? Risk { get; init; }
    public string? Previous { get; init; }
    public string? PreviousConcernsText { get; init; }
    public string? Current { get; init; }
    public string? CurrentConcernsText { get; init; }
}

/// <summary>
/// Risks other than to self. Values are free-text/enum ("YES", "NO", "DK", "NA") as returned by the HMPPS External API.
/// </summary>
public class OtherRisksDto
{
    public string? EscapeOrAbscond { get; init; }
    public string? ControlIssuesDisruptiveBehaviour { get; init; }
    public string? BreachOfTrust { get; init; }
    public string? RiskToOtherPrisoners { get; init; }
}

public class RiskSummaryDto
{
    public string? WhoIsAtRisk { get; init; }
    public string? NatureOfRisk { get; init; }
    public string? RiskImminence { get; init; }
    public string? RiskIncreaseFactors { get; init; }
    public string? RiskMitigationFactors { get; init; }

    /// <summary>
    /// One of "VERY_HIGH", "HIGH", "MEDIUM", "LOW".
    /// </summary>
    public string? OverallRiskLevel { get; init; }

    /// <summary>
    /// Risk level per category (e.g. "children", "public", "knownAdult", "staff", "prisoners") if the offender were released into the community.
    /// </summary>
    public Dictionary<string, string>? RiskInCommunity { get; init; }

    /// <summary>
    /// Risk level per category (e.g. "children", "public", "knownAdult", "staff", "prisoners") while in custody.
    /// </summary>
    public Dictionary<string, string>? RiskInCustody { get; init; }
}
