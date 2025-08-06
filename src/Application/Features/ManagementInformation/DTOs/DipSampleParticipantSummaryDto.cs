namespace Cfo.Cats.Application.Features.ManagementInformation.DTOs;

public record DipSampleParticipantSummaryDto(
    string ParticipantId,
    string ParticipantFullName,
    string? ParticipantOwner,
    string LocationType,
    string? CurrentLocationName,
    string? EnrolmentLocationName,
    ComplianceAnswer CsoComplianceAnswer,
    ComplianceAnswer CpmComplianceAnswer,
    DateTime? ReviewedOn = null,
    string? ReviewedBy = null)
{
    public ComplianceAnswer ComplianceAnswer => 
        CpmComplianceAnswer.IsAnswer ? CpmComplianceAnswer : CsoComplianceAnswer;
}