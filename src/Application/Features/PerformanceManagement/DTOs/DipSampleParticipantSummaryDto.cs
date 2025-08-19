namespace Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

public record DipSampleParticipantSummaryDto(
    string ParticipantId,
    string ParticipantFullName,
    string? ParticipantOwner,
    string LocationType,
    string? CurrentLocationName,
    string? EnrolmentLocationName,
    ComplianceAnswer CsoComplianceAnswer,
    ComplianceAnswer CpmComplianceAnswer,
    ComplianceAnswer FinalComplianceAnswer,
    DateTime? ReviewedOn = null,
    string? ReviewedBy = null)
{
    public bool CsoAndCpmDisagree =>
        CpmComplianceAnswer.IsAnswer && CsoComplianceAnswer.IsAnswer 
        && CpmComplianceAnswer.IsAccepted != CsoComplianceAnswer.IsAccepted;

    public ComplianceAnswer ComplianceAnswer
    {
        get
        {
            if(FinalComplianceAnswer.IsAnswer)
            {
                return FinalComplianceAnswer;
            }

            if(CpmComplianceAnswer.IsAnswer)
            {
                return CpmComplianceAnswer;
            }

            return CsoComplianceAnswer;
        }
    }
}