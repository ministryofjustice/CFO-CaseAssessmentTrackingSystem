using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetParticipantsLatestEngagement
{
    /// <summary>
    /// The date on or after which an engagement is considered recent. Engagements before this
    /// date (or the absence of any engagement) are considered inactive.
    /// </summary>
    public static DateOnly RecencyThreshold => DateOnly.FromDateTime(DateTime.Today).AddMonths(-3);

    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : PaginationFilter, IQuery<Result<PaginatedData<ParticipantEngagementDto>>>
    {
        public required UserProfile CurrentUser { get; init; }
        public bool JustMyCases { get; init; }
        public bool HideRecentEngagements { get; set; }

        /// <summary>Optional filter to a specific current location (by location id).</summary>
        public int? LocationId { get; set; }

        /// <summary>Optional filter to a specific engagement type (the engagement category).</summary>
        public string? EngagementType { get; set; }

        /// <summary>Optional narrowing to a sub-tenant within the current user's visible hierarchy.</summary>
        public string? TenantId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IQueryHandler<Query, Result<PaginatedData<ParticipantEngagementDto>>>
    {
        public async Task<Result<PaginatedData<ParticipantEngagementDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var db = unitOfWork.DbContext;
            var threeMonthsAgo = RecencyThreshold;

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
                where string.IsNullOrWhiteSpace(request.Keyword)
                      || participant.FirstName.Contains(request.Keyword)
                      || participant.LastName.Contains(request.Keyword)
                      || participant.Id.Contains(request.Keyword)
                      || (engagement != null &&
                          (
                              engagement.Description.Contains(request.Keyword) ||
                              engagement.Category.Contains(request.Keyword) ||
                              engagement.EngagedAtLocation.Contains(request.Keyword)
                          ))
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
                    engagement.EngagedOn
                };
#pragma warning restore CS8602, CS8604

            var count = await query.CountAsync(cancellationToken);

            var engagements = await query
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
                    e.EngagedOn
                )).ToListAsync(cancellationToken);

            return Result<PaginatedData<ParticipantEngagementDto>>.Success(
                new PaginatedData<ParticipantEngagementDto>(engagements, count, request.PageNumber, request.PageSize));
        }
    }
}
