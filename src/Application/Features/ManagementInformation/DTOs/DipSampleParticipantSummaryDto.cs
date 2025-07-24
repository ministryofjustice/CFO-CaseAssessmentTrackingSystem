namespace Cfo.Cats.Application.Features.ManagementInformation.DTOs;

public record DipSampleParticipantSummaryDto(
    string ParticipantId,
    string ParticipantFullName,
    string? ParticipantOwner,
    string LocationType,
    string? CurrentLocationName,
    string? EnrolmentLocationName,
    ComplianceAnswer CsoComplianceAnswer,
    DateTime? ReviewedOn = null,
    string? ReviewedBy = null);