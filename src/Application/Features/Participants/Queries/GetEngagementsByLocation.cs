using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetEngagementsByLocation
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : PaginationFilter, IQuery<Result<EngagementsByLocationDto>>
    {
        public required UserProfile CurrentUser { get; init; }
        public bool JustMyCases { get; init; }
        public int? LocationId { get; set; }
        public string? EngagementType { get; set; }
        public string? TenantId { get; set; }
        public required int Month { get; set; }
        public required int Year { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IQueryHandler<Query, Result<EngagementsByLocationDto>>
    {
        public async Task<Result<EngagementsByLocationDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var db = unitOfWork.DbContext;

#pragma warning disable CS8602, CS8604
            var query =
                from participant in db.Participants
                where participant.Owner.TenantId.StartsWith(request.CurrentUser.TenantId)
                where string.IsNullOrWhiteSpace(request.TenantId) || participant.Owner.TenantId.StartsWith(request.TenantId)
                where request.JustMyCases == false || participant.Owner.Id == request.CurrentUser.UserId
                where participant.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value
                join engagement in db.ParticipantEngagements
                    on participant.Id equals engagement.ParticipantId
                join owner in db.Users on participant.OwnerId equals owner.Id
                join currentLocation in db.Locations on participant.CurrentLocation.Id equals currentLocation.Id
                join engagementLocation in db.Locations on engagement.EngagedAtLocation equals engagementLocation.Name
                where request.LocationId == null || engagementLocation.Id == request.LocationId
                where string.IsNullOrWhiteSpace(request.EngagementType) || (engagement != null && engagement.Category == request.EngagementType)
                where engagement.EngagedOn.Month == request.Month && engagement.EngagedOn.Year == request.Year
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
                    EngagedOn = (DateOnly?)engagement.EngagedOn,
                    engagement.EngagedAtLocationType
                };
#pragma warning restore CS8602, CS8604

            var records = await query
                .Where(x => x.EngagedAtLocation != null && x.Category != null)
                .GroupBy(x => new { LocationName = x.EngagedAtLocation!, Category = x.Category! })
                .Select(g => new EngagementLocationCategoryCountDto(g.Key.LocationName, g.Key.Category, g.Count()))
                .ToArrayAsync(cancellationToken);

            var ordered = records
                .OrderBy(x => x.LocationName)
                .ThenBy(x => x.Category)
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
                    e.EngagedOn,
                    e.EngagedAtLocationType))
                .ToListAsync(cancellationToken);

            var details = new PaginatedData<ParticipantEngagementDto>(items, count, request.PageNumber, request.PageSize);

            return Result<EngagementsByLocationDto>.Success(new EngagementsByLocationDto(ordered, details));
        }
    }
}
