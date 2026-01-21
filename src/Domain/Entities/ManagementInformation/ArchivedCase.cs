namespace Cfo.Cats.Domain.Entities.ManagementInformation;

public class ArchivedCase
{
#pragma warning disable CS8618
    private ArchivedCase()
    {
    }
#pragma warning restore CS8618

    public static ArchivedCase CreateArchivedCase(
        string participantId,
        int enrolmentHistoryId,
        DateTime occurredOn,
        string tenantId,
        string supportWorker,
        string contractId,
        int locationId,
        string locationType,
        string? archiveReason)
    {
        return new ArchivedCase
        {
            Id = Guid.CreateVersion7(),
            ParticipantId = participantId,
            EnrolmentHistoryId = enrolmentHistoryId,
            OccurredOn = occurredOn,
            TenantId = tenantId,
            SupportWorker = supportWorker!,
            CreatedOn = DateTime.UtcNow,
            ContractId = contractId,
            LocationId = locationId,
            LocationType = locationType,
            ArchiveReason = archiveReason
        };
    }

    public required Guid Id { get; set; }
    public required string ParticipantId { get; set; }
    public required int EnrolmentHistoryId { get; set; }
    public required DateTime OccurredOn { get; set; }
    public required DateTime CreatedOn { get; set; }
    public required string SupportWorker { get; set; }
    public required string ContractId { get; set; }
    public required int LocationId { get; set; }
    public required string LocationType { get; set; }
    public required string TenantId { get; set; }
    public required string? ArchiveReason { get; set; }
}