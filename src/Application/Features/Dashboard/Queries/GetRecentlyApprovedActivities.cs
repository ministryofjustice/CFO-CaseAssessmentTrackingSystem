using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Features.Locations.DTOs;
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
        public DateTime? CommencedStart { get; set; }
        public DateTime? CommencedEnd { get; set; }
        public LocationDto? Location { get; set; }
        public List<ActivityType>? IncludeTypes { get; set; }
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
                        select a;
            
            // Apply optional filters
            if (request.CommencedStart.HasValue)
            {
                query = query.Where(a => a.CommencedOn >= request.CommencedStart.Value);
            }
            
            if (request.CommencedEnd.HasValue)
            {
                query = query.Where(a => a.CommencedOn <= request.CommencedEnd.Value);
            }
            
            if (request.Location is not null)
            {
                query = query.Where(a => a.TookPlaceAtLocation.Id == request.Location.Id);
            }
            
            if (request.IncludeTypes is { Count: > 0 })
            {
                var typeValues = request.IncludeTypes.Select(t => t.Value).ToList();
                query = query.Where(a => typeValues.Contains(a.Type));
            }
            
            var results = from a in query
                         select new RecentlyApprovedActivitiesSummaryDto
                         {
                             ParticipantId = a.Participant.Id,
                             Participant = $"{a.Participant.FirstName} {a.Participant.LastName}",
                             Definition = a.Definition,
                             ApprovedOn = a.CompletedOn
                         };
#pragma warning restore CS8602

            return await results
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
