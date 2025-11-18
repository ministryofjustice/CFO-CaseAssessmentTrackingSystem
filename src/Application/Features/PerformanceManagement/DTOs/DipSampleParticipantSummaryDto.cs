using Microsoft.AspNetCore.Authorization.Infrastructure;

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
    public bool CsoAndCpmDisagree => (CsoComplianceAnswer, CpmComplianceAnswer) switch
    {
        ({ IsAnswer: false }, _) => false, // CSO Has not yet answered
        (_, { IsAnswer: false }) => false, // CPM has not answered yet
        ({ Name: "Unsure" }, { IsAnswer: true }) => true, // CSO Unsure is classed as a disagreement  
        ({ IsAnswer: true }, { IsAuto: true }) => false, // CPM Accepts CSO Answer
        ({ IsAccepted: true }, { IsAccepted: true }) => false, // Both accepted
        ({ IsAccepted: false }, { IsAccepted: false }) => false, // Both rejected
        _ => true,
    };

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