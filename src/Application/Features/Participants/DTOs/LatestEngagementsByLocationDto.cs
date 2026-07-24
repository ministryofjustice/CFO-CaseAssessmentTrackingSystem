using Cfo.Cats.Application.Common.Models;

namespace Cfo.Cats.Application.Features.Participants.DTOs;

/// <summary>
/// A breakdown of participants' latest engagements for a single current location, split by recency.
/// </summary>
/// <param name="LocationName">The participant's current location.</param>
/// <param name="RecentCount">Participants whose latest engagement was within the last 3 months.</param>
/// <param name="InactiveCount">Participants whose latest engagement was more than 3 months ago, or who have never engaged.</param>
public record LocationEngagementSummaryDto(string LocationName, int RecentCount, int InactiveCount)
{
    public int Total => RecentCount + InactiveCount;
}

/// <summary>
/// A location/category combination and how many participants' latest engagement falls into that category at that location.
/// </summary>
/// <param name="LocationName">The location (current or engaged-at, depending on the request).</param>
/// <param name="Category">The engagement category.</param>
/// <param name="Count">Participants whose latest engagement at this location falls into this category.</param>
public record LocationCategoryCountDto(string LocationName, string Category, int Count);

/// <summary>
/// The location-level breakdown of latest engagements with headline totals, alongside a single
/// page of participant detail rows. The per-location summary (used for the chart) is aggregated
/// over the whole filtered result set, while <see cref="Details"/> carries only the current page,
/// so one query serves both the chart and the paged table.
/// </summary>
public record LatestEngagementsByLocationDto(
    LocationEngagementSummaryDto[] Records,
    PaginatedData<ParticipantEngagementDto> Details,
    LocationCategoryCountDto[]? CategoryRecords = null)
{
    public int TotalRecent => Records.Sum(r => r.RecentCount);
    public int TotalInactive => Records.Sum(r => r.InactiveCount);
    public int Total => TotalRecent + TotalInactive;
}
