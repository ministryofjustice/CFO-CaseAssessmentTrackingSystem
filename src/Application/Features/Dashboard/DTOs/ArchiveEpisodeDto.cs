namespace Cfo.Cats.Application.Features.Dashboard.DTOs;

public record ArchiveEpisodeDto
{
    public required string ParticipantId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? ContractId { get; init; }
    public string? ContractName { get; init; }
    public required string LocationName { get; init; }
    public required string LocationType { get; init; }
    public required string ArchiveReason { get; init; }
    public string? ArchiveAdditionalInfo { get; init; }
    public string? UnarchiveReason { get; init; }
    public string? UnarchiveAdditionalInfo { get; init; }
    public required DateTime From { get; init; }
    public DateTime? To { get; init; }
    public string? CreatedBy { get; init; }
}
