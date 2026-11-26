namespace Cfo.Cats.Application.Features.Dashboard.DTOs;

public record ArchiveRecurrencesDto
{
    public ArchiveRecurrencesDto(
        ArchiveEpisodeDto[] episodes,
        CurrentParticipantContractDto[] currentContracts)
    {
        Participants = BuildParticipants(episodes);
        ContractBreakdown = BuildContractBreakdown(Participants, currentContracts);
    }

    private static ArchiveRecurrenceParticipantDto[] BuildParticipants(
        ArchiveEpisodeDto[] episodes)
        => episodes
            .GroupBy(x => x.ParticipantId)
            .Select(group =>
            {
                var ordered = group.OrderBy(x => x.From).ToArray();
                var contracts = ordered
                    .Where(x => !string.IsNullOrWhiteSpace(x.ContractId))
                    .Select(x => x.ContractId!)
                    .Distinct()
                    .ToArray();
                var contractNames = ordered
                    .Select(x => x.ContractName ?? "Contract unknown")
                    .Distinct()
                    .OrderBy(x => x)
                    .ToArray();
                var openCount = ordered.Count(x => x.To is null);

                var totalDays = ordered.Sum(x =>
                    (int)Math.Ceiling(((x.To ?? DateTime.UtcNow) - x.From).TotalDays));

                return new ArchiveRecurrenceParticipantDto
                {
                    ParticipantId = group.Key,
                    ParticipantName = $"{ordered[0].FirstName} {ordered[0].LastName}",
                    EpisodeCount = ordered.Length,
                    CompletedEpisodeCount = ordered.Length - openCount,
                    OpenEpisodeCount = openCount,
                    CrossContract = contracts.Length > 1,
                    ContractScope = contracts.Length > 1
                        ? "Cross-contract"
                        : contracts.Length == 1
                            ? "Same contract"
                            : "Contract unknown",
                    ContractNames = contractNames,
                    LocationNames = ordered
                        .Select(x => x.LocationName)
                        .Distinct()
                        .OrderBy(x => x)
                        .ToArray(),
                    TotalArchivedDays = totalDays,
                    Episodes = ordered
                };
            })
            .OrderByDescending(x => x.EpisodeCount)
            .ThenBy(x => x.ParticipantName)
            .ToArray();

    private static ArchiveContractBreakdownDto[] BuildContractBreakdown(
        ArchiveRecurrenceParticipantDto[] participants,
        CurrentParticipantContractDto[] currentContracts)
    {
        var currentContractByParticipant = currentContracts
            .ToDictionary(x => x.ParticipantId);

        return participants
            .GroupBy(participant =>
                currentContractByParticipant[participant.ParticipantId].ContractId)
            .Select(contractParticipants =>
            {
                var contractId = contractParticipants.Key;
                var contractName = contractParticipants
                    .Select(x => currentContractByParticipant[x.ParticipantId].ContractName)
                    .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x))
                    ?? "Contract unknown";
                var contractEpisodes = contractParticipants
                    .SelectMany(x => x.Episodes)
                    .ToArray();

                return new ArchiveContractBreakdownDto
                {
                    ContractName = contractName,
                    EpisodeCount = contractEpisodes.Length,
                    ParticipantCount = contractParticipants.Count(),
                    OpenEpisodeCount = contractEpisodes.Count(x => x.To is null),
                    CrossContractEpisodeCount = contractEpisodes.Count(x => x.ContractId != contractId)
                };
            })
            .OrderByDescending(x => x.EpisodeCount)
            .ThenBy(x => x.ContractName)
            .ToArray();
    }

    public ArchiveRecurrenceParticipantDto[] Participants { get; }
    public ArchiveContractBreakdownDto[] ContractBreakdown { get; }
}
