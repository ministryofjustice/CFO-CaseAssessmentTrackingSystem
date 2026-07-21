using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

/// <summary>
/// Returns the distinct engagement type (category) values that exist within the current user's
/// tenant, so the dashboard's Engagement Type filter is sourced from real data rather than hard-coded.
/// </summary>
public static class GetEngagementTypes
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IQuery<Result<string[]>>
    {
        public required UserProfile CurrentUser { get; init; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IQueryHandler<Query, Result<string[]>>
    {
        public async Task<Result<string[]>> Handle(Query request, CancellationToken cancellationToken)
        {
            var db = unitOfWork.DbContext;

#pragma warning disable CS8602, CS8604
            var types = await (
                from engagement in db.ParticipantEngagements
                join participant in db.Participants on engagement.ParticipantId equals participant.Id
                where participant.Owner.TenantId.StartsWith(request.CurrentUser.TenantId)
                select engagement.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToArrayAsync(cancellationToken);
#pragma warning restore CS8602, CS8604

            return Result<string[]>.Success(types);
        }
    }
}
