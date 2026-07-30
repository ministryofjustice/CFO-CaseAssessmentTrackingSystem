using Cfo.Cats.Application.Common.Models;

namespace Cfo.Cats.Application.Features.Participants.DTOs;

/// <summary>
/// A single engagement location / category combination and how many participants' latest
/// engagement falls into it. Used instead of a fixed-column DTO because the set of categories
/// is open-ended and data-driven, rather than a small fixed set that can be named as properties.
/// </summary>
/// <param name="LocationName">The location at which the engagement took place.</param>
/// <param name="Category">The engagement category.</param>
/// <param name="Count">Participants whose latest engagement at this location falls into this category.</param>
public record EngagementLocationCategoryCountDto(string LocationName, string Category, int Count)
{
    public static string GetCategoryColour(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return "#4e4e4e";
        }
        
        try
        {
            return category switch
            {
                "Assessment"     => "#22C55E",
                "Hub Induction"  => "#E11D48",
                "Wing Induction" => "#6366F1",
                _ => ActivityType.FromName(category).Colour
            };
        }
        catch
        {
            return "#808080";
        }
    }
}

/// <summary>
/// The engagement-location breakdown of latest engagements by category, alongside a single page
/// of participant detail rows. The per-location summary (used for the chart) is aggregated over
/// the whole filtered result set, while <see cref="Details"/> carries only the current page, so
/// one query serves both the chart and the paged table.
/// </summary>
public record EngagementsByLocationDto(
    EngagementLocationCategoryCountDto[] Records,
    PaginatedData<ParticipantEngagementDto> Details)
{
    public int Total => Records.Sum(r => r.Count);
}
