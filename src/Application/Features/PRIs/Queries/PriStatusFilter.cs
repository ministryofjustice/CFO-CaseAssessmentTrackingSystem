namespace Cfo.Cats.Application.Features.PRIs.Queries;

/// <summary>
/// Filters PRIs by lifecycle status. Active covers all statuses where <c>PriStatus.IsActive</c> is true.
/// </summary>
public enum PriStatusFilter
{
    Active,
    Completed,
    Abandoned,
    All
}
