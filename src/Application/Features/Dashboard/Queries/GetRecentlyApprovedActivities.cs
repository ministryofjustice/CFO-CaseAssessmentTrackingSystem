using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetRecentlyApprovedActivities
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<List<RecentlyApprovedActivitiesSummaryDto>>
    {
        public required UserProfile UserProfile { get; set; }
        public int DaysBack { get; set; } = 30;
    }

    public class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, List<RecentlyApprovedActivitiesSummaryDto>>
    {
        public async Task<List<RecentlyApprovedActivitiesSummaryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var db = unitOfWork.DbContext;
            var cutoffDate = DateTime.UtcNow.AddDays(-request.DaysBack);

#pragma warning disable CS8602
            var query = from a in db.Activities
                        where a.TenantId.StartsWith(request.UserProfile.TenantId!)
                            && a.OwnerId == request.UserProfile.UserId
                            && a.Status == ActivityStatus.ApprovedStatus.Value
                            && a.CompletedOn >= cutoffDate
                        select new RecentlyApprovedActivitiesSummaryDto
                        {
                            ParticipantId = a.Participant.Id,
                            Participant = $"{a.Participant.FirstName} {a.Participant.LastName}",
                            Definition = a.Definition,
                            ApprovedOn = a.CompletedOn
                        };
#pragma warning restore CS8602

            return await query
                .OrderByDescending(a => a.ApprovedOn)
                .Take(10)
                .ToListAsync(cancellationToken);
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator() =>
                RuleFor(x => x.UserProfile.UserId)
                    .NotNull();
        }
    }
}
