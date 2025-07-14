namespace Cfo.Cats.Application.Features.ManagementInformation.DTOs;

public record DipSampleParticipantSummaryDto(
    string ParticipantId,
    string ParticipantFullName,
    string? ParticipantOwner,
    string LocationType,
    string? EnrolmentLocationName,
    bool? IsCompliant = null,
    DateTime? ReviewedOn = null,
    string? ReviewedBy = null);