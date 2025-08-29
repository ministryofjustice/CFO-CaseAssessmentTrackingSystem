namespace Cfo.Cats.Application.Features.Participants.DTOs;

public record ParticipantEngagementDto(
    string ParticipantId, 
    string FullName,
    string Category, 
    string Description,
    string EngagedAtLocationName,
    string EngagedAtContractName,
    string EngagedWithDisplayName, 
    string EngagedWithTenantName,
    string SupportWorkerDisplayName,
    DateOnly EngagedOn)
{
    public bool HasNotEngagedRecently => EngagedOn < DateOnly.FromDateTime(DateTime.Today).AddMonths(-3);
    public bool LastEngagedWithCurrentSupportWorker => SupportWorkerDisplayName == EngagedWithDisplayName;
}
