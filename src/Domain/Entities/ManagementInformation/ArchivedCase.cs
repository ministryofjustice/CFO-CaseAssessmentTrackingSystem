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
        string firstName,
        string lastName,
        int enrolmentHistoryId,
        DateTime created,
        string createdBy,
        string? additionalInfo,
        string? archiveReason,
        string? unarchiveAdditionalInfo,
        string? unarchiveReason,
        DateTime from,
        DateTime? to,
        string? contractId,
        int locationId,
        string locationType,
        string tenantId) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            ParticipantId = participantId,
            FirstName = firstName,
            LastName = lastName,
            EnrolmentHistoryId = enrolmentHistoryId,
            Created = created,
            CreatedBy = createdBy,
            AdditionalInfo = additionalInfo,
            ArchiveReason = archiveReason,
            UnarchiveAdditionalInfo = unarchiveAdditionalInfo,
            UnarchiveReason = unarchiveReason,
            From = from,
            To = to,
            ContractId = contractId,
            LocationId = locationId,
            LocationType = locationType,
            TenantId = tenantId,
        };

    public void Close(DateTime to, string? unarchiveAdditionalInfo, string? unarchiveReason)
    {
        if (To is not null)
        {
            throw new ApplicationException("Cannot close archived case more than once.");
        }

        if (to < From)
        {
            throw new ArgumentException("To cannot be earlier than From");
        }

        To = to;
        UnarchiveReason = unarchiveReason;
        UnarchiveAdditionalInfo = unarchiveAdditionalInfo;
    }

    public required Guid Id { get; init; }
    public required string ParticipantId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required int EnrolmentHistoryId { get; init; }
    public required DateTime Created { get; init; }
    public required string CreatedBy { get; init; }
    public required string? AdditionalInfo { get; init; }
    public required string? ArchiveReason { get; init; }
    public required string? UnarchiveAdditionalInfo { get; set; }
    public required string? UnarchiveReason { get; set; }
    public required DateTime From { get; init; }
    public required DateTime? To { get; set; }
    public required string? ContractId { get; init; }
    public required int LocationId { get; init; }
    public required string LocationType { get; init; }
    public required string TenantId { get; init; }
}