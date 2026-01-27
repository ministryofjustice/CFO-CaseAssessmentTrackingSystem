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
        DateTime from,
        DateTime? to,
        string contractId,
        int locationId,
        string locationType,
        string tenantId)
    {
        return new ArchivedCase
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
            From = from,
            To = to,
            ContractId = contractId,
            LocationId = locationId,
            LocationType = locationType,
            TenantId = tenantId,
        };
    }

    public ArchivedCase SetTo(DateTime to)
    {
        if (To is not null)
        {
            throw new ApplicationException("Cannot set the to date more than once.");
        }

        if (to < From)
        {
            throw new ArgumentException("To cannot be earlier that From");
        }

        To = to;
        return this;
    }

    public required Guid Id { get; set; }
    public required string ParticipantId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required int EnrolmentHistoryId { get; set; }
    public required DateTime Created { get; set; }
    public required string CreatedBy { get; set; }
    public required string? AdditionalInfo { get; set; }
    public required string? ArchiveReason { get; set; }
    public required DateTime From { get; set; }
    public required DateTime? To { get; set; }
    public required string ContractId { get; set; }
    public required int LocationId { get; set; }
    public required string LocationType { get; set; }
    public required string TenantId { get; set; }
}