using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetRecentlyApprovedActivities
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<PaginatedData<RecentlyApprovedActivitiesSummaryDto>>>
    {
        public required UserProfile UserProfile { get; init; }
        public int DaysBack { get; init; } = 30;
        public DateTime? CommencedStart { get; init; }
        public DateTime? CommencedEnd { get; init; }
        public LocationDto? Location { get; init; }
        public List<ActivityType>? IncludeTypes { get; init; }
        public bool JustMyParticipants { get; init; } = true;
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        public string OrderBy { get; set; } = "ApprovedOn";
        public string SortDirection { get; init; } = "Descending";
    }

    public class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<PaginatedData<RecentlyApprovedActivitiesSummaryDto>>>
    {
        public async Task<Result<PaginatedData<RecentlyApprovedActivitiesSummaryDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var db = unitOfWork.DbContext;
            var cutoffDate = DateTime.UtcNow.AddDays(-request.DaysBack);

#pragma warning disable CS8602
            var query = from a in db.Activities
                        where a.TenantId.StartsWith(request.UserProfile.TenantId!)
                            && a.Status == ActivityStatus.ApprovedStatus.Value
                            && a.CompletedOn >= cutoffDate
                        select a;
            
            if (request.JustMyParticipants)
            {
                query = query.Where(a => a.OwnerId == request.UserProfile.UserId);
            }
            
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
                             ApprovedOn = a.CompletedOn,
                             Created = a.Created,
                             CommencedOn = a.CommencedOn,
                             TookPlaceAtLocationName = a.TookPlaceAtLocation.Name
                         };
            
            results = request.SortDirection.ToLower() == "ascending"
                ? results.OrderBy(a => a.ApprovedOn)
                : results.OrderByDescending(a => a.ApprovedOn);
#pragma warning restore CS8602

            var count = await results.CountAsync(cancellationToken);
            
            var items = await results
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return Result<PaginatedData<RecentlyApprovedActivitiesSummaryDto>>.Success(
                new PaginatedData<RecentlyApprovedActivitiesSummaryDto>(items, count, request.PageNumber, request.PageSize));
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(x => x.UserProfile.UserId)
                    .NotNull();
                
                RuleFor(r => r.PageNumber)
                    .GreaterThan(0)
                    .WithMessage(string.Format(ValidationConstants.PositiveNumberMessage, "Page Number"));

                RuleFor(r => r.PageSize)
                    .GreaterThan(0)
                    .LessThanOrEqualTo(ValidationConstants.MaximumPageSize)
                    .WithMessage(ValidationConstants.MaximumPageSizeMessage);
            }
        }
    }
}
