namespace Cfo.Cats.Application.Features.Dashboard.DTOs;

/// <summary>
/// Represents a count of risk renewals based on their current state.
/// </summary>
/// <param name="Description">The description of whom the risk record applies to.</param>
/// <param name="Overdue">The count of risk renewals that have expired</param>
/// <param name="Upcoming">The count of risk renewals coming up.</param>
public record RiskDueAggregateDto (string Description, int Overdue, int Upcoming);