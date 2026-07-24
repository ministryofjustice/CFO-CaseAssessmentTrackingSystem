using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

/// <summary>
/// Groups each participant's latest engagement by their current location, split into recent
/// (engaged within the last 3 months) versus inactive (engaged more than 3 months ago, or never).
/// The per-location summary is aggregated in the database over the whole filtered set, while the
/// participant detail rows are paged, so a single request serves both the chart and the table.
/// </summary>
public static class GetLatestEngagementsByLocation
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : PaginationFilter, IQuery<Result<LatestEngagementsByLocationDto>>
    {
        public required UserProfile CurrentUser { get; init; }
        public bool JustMyCases { get; init; }
        public bool HideRecentEngagements { get; set; }
        public int? LocationId { get; set; }
        public string? EngagementType { get; set; }
        public string? TenantId { get; set; }
        public LocationGroupingMode GroupBy { get; set; } = LocationGroupingMode.CurrentLocation;
    }

    public enum LocationGroupingMode
    {
        CurrentLocation,
        EngagedAtLocation
    }

    public class Handler(IUnitOfWork unitOfWork) : IQueryHandler<Query, Result<LatestEngagementsByLocationDto>>
    {
        public async Task<Result<LatestEngagementsByLocationDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var db = unitOfWork.DbContext;
            var threeMonthsAgo = GetParticipantsLatestEngagement.RecencyThreshold;

#pragma warning disable CS8602, CS8604
            var query =
                from participant in db.Participants
                where participant.Owner.TenantId.StartsWith(request.CurrentUser.TenantId)
                where string.IsNullOrWhiteSpace(request.TenantId) || participant.Owner.TenantId.StartsWith(request.TenantId)
                where request.JustMyCases == false || participant.Owner.Id == request.CurrentUser.UserId
                where participant.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value
                join engagement in db.ParticipantEngagements
                    on participant.Id equals engagement.ParticipantId into leftJoin
                from engagement in leftJoin
                    .OrderByDescending(pe => pe.EngagedOn)
                    .ThenByDescending(pe => pe.CreatedOn)
                    .Take(1)
                    .DefaultIfEmpty()
                join owner in db.Users on participant.OwnerId equals owner.Id
                join currentLocation in db.Locations on participant.CurrentLocation.Id equals currentLocation.Id
                where request.LocationId == null || currentLocation.Id == request.LocationId
                where string.IsNullOrWhiteSpace(request.EngagementType) || (engagement != null && engagement.Category == request.EngagementType)
                where request.HideRecentEngagements == false || (engagement == null || engagement.EngagedOn < threeMonthsAgo)
                select new
                {
                    participant.Id,
                    FullName = participant.FirstName + " " + participant.LastName,
                    engagement.Category,
                    engagement.Description,
                    engagement.EngagedAtLocation,
                    engagement.EngagedAtContract,
                    engagement.EngagedWith,
                    engagement.EngagedWithTenant,
                    owner.DisplayName,
                    CurrentLocationName = currentLocation.Name,
                    EngagedOn = (DateOnly?)engagement.EngagedOn
                };
#pragma warning restore CS8602, CS8604

            // Aggregate the whole filtered set for the chart / headline totals.
            var records = await query
                .GroupBy(x => request.GroupBy == LocationGroupingMode.EngagedAtLocation ? (x.EngagedAtLocation ?? "Unknown Location") : x.CurrentLocationName)
                .Select(g => new LocationEngagementSummaryDto(
                    g.Key,
                    g.Count(x => x.EngagedOn != null && x.EngagedOn >= threeMonthsAgo),
                    g.Count(x => x.EngagedOn == null || x.EngagedOn < threeMonthsAgo)))
                .ToArrayAsync(cancellationToken);

            var ordered = records
                .OrderBy(x => x.LocationName)
                .ToArray();

            // Page the detail rows for the table.
            var count = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy($"{request.OrderBy} {request.SortDirection}")
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(e => new ParticipantEngagementDto(
                    e.Id,
                    e.FullName,
                    e.Category,
                    e.Description,
                    e.EngagedAtLocation,
                    e.EngagedAtContract,
                    e.EngagedWith,
                    e.EngagedWithTenant,
                    e.DisplayName,
                    e.CurrentLocationName,
                    e.EngagedOn))
                .ToListAsync(cancellationToken);

            var details = new PaginatedData<ParticipantEngagementDto>(items, count, request.PageNumber, request.PageSize);

            return Result<LatestEngagementsByLocationDto>.Success(new LatestEngagementsByLocationDto(ordered, details));
        }
    }
}
