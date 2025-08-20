namespace Cfo.Cats.Application.Features.Participants.DTOs;

public record ParticipantEngagementDto(string ParticipantId, string FullName, string Category, string Description, string SupportWorkerDisplayName, DateOnly LastEngagedOn)
{
    public bool HasNotEngagedRecently => LastEngagedOn < DateOnly.FromDateTime(DateTime.Today).AddMonths(-3);
}
