namespace Cfo.Cats.Server.UI.Pages.Workspaces.Performance.Components;

public record ArchiveRecurrencesSessionData
{
    public required DateTime? StartDate { get; init; }
    public required DateTime? EndDate { get; init; }
    public required int Threshold { get; init; }
    public required string? ContractId { get; init; }

    public static ArchiveRecurrencesSessionData FromState(
        DateRange dateRange,
        int threshold,
        string? contractId)
        => new()
        {
            StartDate = dateRange.Start,
            EndDate = dateRange.End,
            Threshold = threshold,
            ContractId = contractId
        };
}
