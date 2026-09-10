using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

/// <summary>
/// Scopes the "Engaged With" user picker on the Latest Engagements By Location dashboard to only
/// the names that actually appear in the currently visible table, mirroring
/// <see cref="GetLatestEngagementsByLocation"/> exactly: only each participant's latest engagement
/// is considered (not their full history), and every filter the page applies (tenant, just-my-cases,
/// location, engagement type, hide-recent) is applied here too.
/// <see cref="Domain.Entities.ManagementInformation.ParticipantEngagement.EngagedWith"/> is a
/// free-text name captured at the time of the engagement rather than a user id, so this returns
/// distinct names rather than user ids.
/// </summary>
public static class GetEngagementAssignees
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query(UserProfile currentUser) : IQuery<Result<string[]>>
    {
        public UserProfile CurrentUser { get; } = currentUser;
        public bool JustMyCases { get; init; }
        public bool HideRecentEngagements { get; init; }
        public int? LocationId { get; init; }
        public string? EngagementType { get; init; }
        public string? TenantId { get; init; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IQueryHandler<Query, Result<string[]>>
    {
        public async Task<Result<string[]>> Handle(Query request, CancellationToken cancellationToken)
        {
            var db = unitOfWork.DbContext;
            var threeMonthsAgo = GetParticipantsLatestEngagement.RecencyThreshold;

#pragma warning disable CS8602, CS8604
            var names = await (
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
                join currentLocation in db.Locations on participant.CurrentLocation.Id equals currentLocation.Id
                where request.LocationId == null || currentLocation.Id == request.LocationId
                where string.IsNullOrWhiteSpace(request.EngagementType) || (engagement != null && engagement.Category == request.EngagementType)
                where request.HideRecentEngagements == false || (engagement == null || engagement.EngagedOn < threeMonthsAgo)
                where engagement != null && !string.IsNullOrWhiteSpace(engagement.EngagedWith)
                select engagement.EngagedWith)
#pragma warning restore CS8602, CS8604
                .Distinct()
                .OrderBy(n => n)
                .ToArrayAsync(cancellationToken);

            return Result<string[]>.Success(names);
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator() =>
            RuleFor(x => x.CurrentUser)
                .NotNull();
    }
}
