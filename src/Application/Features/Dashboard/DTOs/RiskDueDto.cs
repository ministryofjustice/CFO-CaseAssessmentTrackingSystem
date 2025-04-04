namespace Cfo.Cats.Application.Features.Dashboard.DTOs;

public record RiskDueDto(string ParticipantId, string FirstName, string LastName, DateTime DueDate, DateTime? LastRiskUpdate, EnrolmentStatus EnrolmentStatus);
