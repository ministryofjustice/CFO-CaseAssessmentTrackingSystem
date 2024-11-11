namespace Cfo.Cats.Application.Features.Dashboard.DTOs;

public class ParticipantCountSummaryDto
{
    public int IdentifiedCases { get; set; }
    public int EnrollingCases { get; set; }
    public int CasesAtPqa { get; set; }
    public int CasesAtCfo { get; set; }
    public int ApprovedCases { get; set; }
}

