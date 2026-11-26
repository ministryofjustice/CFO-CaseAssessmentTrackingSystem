namespace Cfo.Cats.Application.Features.Dashboard.DTOs;

public record ArchiveContractBreakdownDto
{
    public required string ContractName { get; init; }
    public required int EpisodeCount { get; init; }
    public required int ParticipantCount { get; init; }
    public required int OpenEpisodeCount { get; init; }
    public required int CrossContractEpisodeCount { get; init; }
}
