using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Specifications;

namespace Cfo.Cats.Application.Features.Participants.Queries.Extensions;

public static class ParticipantSortingExtensions
{
    public static IQueryable<ParticipantPaginationDto> ApplySmartSorting(
        this IQueryable<ParticipantPaginationDto> query,
        string orderBy,
        string sortDirection,
        RecentParticipantFilter recentAction,
        DateTime? riskDue)
    {
        // 1. Determine if the user is relying on the default sort
        var isDefaultSort = string.IsNullOrWhiteSpace(orderBy) || 
                            orderBy.Trim().Equals("id", StringComparison.OrdinalIgnoreCase);

        // 2. Map the requested sort column
        var sortColumn = string.IsNullOrWhiteSpace(orderBy) ? "Id" : orderBy.Trim().ToLowerInvariant() switch
        {
            "firstname" => "FirstName",
            "enrolmentstatus" => "EnrolmentStatus",
            "consentstatus" => "ConsentStatus",
            "currentlocation" => "CurrentLocation.Name",
            "enrolmentlocation" => "EnrolmentLocation.Name",
            "owner" => "Owner",
            "tenant" => "Tenant",
            "riskdue" => "RiskDue",
            "lastname" => "LastName",
            "assignedon" => "AssignedOn",
            "accessedon" => "AccessedOn",
            "archivedon" => "ArchivedOn",
            _ => "Id"
        };

        // 3. Map the requested sort direction
        var direction = string.IsNullOrWhiteSpace(sortDirection) || 
                        sortDirection.Equals("Ascending", StringComparison.OrdinalIgnoreCase)
            ? "ascending"
            : "descending";

        // 4. Apply Smart Defaults if the user hasn't explicitly requested a specific sort
        if (isDefaultSort)
        {
            if (recentAction == RecentParticipantFilter.VisitedLast7Days)
            {
                sortColumn = "AccessedOn";
                direction = "descending";
            }
            else if (recentAction == RecentParticipantFilter.AssignedLast10Days || 
                     recentAction == RecentParticipantFilter.AssignedLast30Days)
            {
                sortColumn = "AssignedOn";
                direction = "descending";
            }
            else if (recentAction == RecentParticipantFilter.ArchivedLast30Days)
            {
                sortColumn = "ArchivedOn";
                direction = "descending";
            }
            else if (riskDue.HasValue)
            {
                sortColumn = "RiskDue";
                direction = "ascending"; 
            }
        }

        return query.OrderBy($"{sortColumn} {direction}");
    }
}
