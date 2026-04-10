namespace Cfo.Cats.Application.Features.Compliance.DTOs;

public record RiskComplianceSummaryDto
{
    public required string Contract { get; set;}
    public int Total { get; set;}
    public int LicenceEndDateCompleted { get; set; }
    public int LicenceConditionsCompleted { get; set; }
    public int RiskSetWithin4Weeks { get; set; }
    public int ProbationPractitionerDetailsDocumented { get; set; }
    public int NoUnknownRiskSet { get; set; }
}

