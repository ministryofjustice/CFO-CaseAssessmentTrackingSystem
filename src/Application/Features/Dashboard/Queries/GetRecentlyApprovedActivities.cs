using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetRecentlyApprovedActivities
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<ApprovedActivitiesDto>>
    {
        public required DateTime StartDate { get; init; }
        public required DateTime EndDate { get; init; }
        public string? UserId { get; init; }
        public string? TenantId { get; init; }
        public required UserProfile CurrentUser { get; init; }
    }

    public class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<ApprovedActivitiesDto>>
    {
        public async Task<Result<ApprovedActivitiesDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var baseQuery = context.Activities
                .Where(a => a.Status == ActivityStatus.ApprovedStatus.Value
                            && a.CompletedOn >= request.StartDate.Date
                            && a.CompletedOn <= request.EndDate.Date);

            // Checks and applies filter based on UserId or TenantId else throws exception
            baseQuery = request switch
            {
                { UserId: var userId } when !string.IsNullOrWhiteSpace(userId)
                    => baseQuery.Where(a => a.OwnerId == userId),

                { TenantId: var tenantId } when !string.IsNullOrWhiteSpace(tenantId)
                    => baseQuery.Where(a => a.TenantId.StartsWith(tenantId)),

                _ => throw new ArgumentException("Invalid request: UserId or TenantId must be provided.")
            };

            var results = await (from a in baseQuery
                          orderby a.CompletedOn descending
                          select new ActivityDetail
                          {
                              ParticipantId = a.ParticipantId,
                              ParticipantName = a.Participant!.FirstName + " " + a.Participant.LastName,
                              Category = a.Definition.Category.Name,
                              ActivityType = a.Definition.Type.Name,
                              ApprovedOn = a.CompletedOn,
                              AddedOn = a.Created,
                              TookPlaceOn = a.CommencedOn,
                              TookPlaceAtLocation = a.TookPlaceAtLocation.Name
                          })
                          .AsNoTracking()
                          .ToArrayAsync(cancellationToken);

            var custody = results.Count(r => r.TookPlaceAtLocation.Contains("HMP") || r.TookPlaceAtLocation.Contains("Wing"));
            var community = results.Length - custody;

            return new ApprovedActivitiesDto(results, custody, community);
        }
    }

    public record ApprovedActivitiesDto(ActivityDetail[] Details, int Custody, int Community);

    public record ActivityDetail
    {
        public required string ParticipantId { get; init; }
        public required string ParticipantName { get; init; }
        public required string Category { get; init; }
        public required string ActivityType { get; init; }
        public DateTime? ApprovedOn { get; init; }
        public DateTime? AddedOn { get; init; }
        public DateTime? TookPlaceOn { get; init; }
        public required string TookPlaceAtLocation { get; init; }
    }
}
