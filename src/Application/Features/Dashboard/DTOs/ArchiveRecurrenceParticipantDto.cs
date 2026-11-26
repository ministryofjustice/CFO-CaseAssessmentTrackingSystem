namespace Cfo.Cats.Application.Features.Dashboard.DTOs;

public record ArchiveRecurrenceParticipantDto
{
    public required string ParticipantId { get; init; }
    public required string ParticipantName { get; init; }
    public required int EpisodeCount { get; init; }
    public required int CompletedEpisodeCount { get; init; }
    public required int OpenEpisodeCount { get; init; }
    public required bool CrossContract { get; init; }
    public required string ContractScope { get; init; }
    public required string[] ContractNames { get; init; }
    public required string[] LocationNames { get; init; }
    public required int TotalArchivedDays { get; init; }
    public required ArchiveEpisodeDto[] Episodes { get; init; }
}
