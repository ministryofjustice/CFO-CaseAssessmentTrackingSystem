using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.Features.Activities.Specifications;
using Cfo.Cats.Application.SecurityConstants;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.Activities.Queries;

public static class AllActivitiesWithPagination
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : AllActivitiesAdvancedFilter, IQuery<Result<PaginatedData<ActivityPaginationDto>>>
    {
        [JsonIgnore]
        public AllActivitiesAdvancedSpecification Specification => new(this);
    }

    public class Handler(IUnitOfWork unitOfWork)
        : IQueryHandler<Query, Result<PaginatedData<ActivityPaginationDto>>>
    {
        public async Task<Result<PaginatedData<ActivityPaginationDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var db = unitOfWork.DbContext;

            var activities = db.Activities.ApplySpecification(request.Specification);

            if (request.ReturnedWithinDays is { } days)
            {
                var cutoff = DateTime.UtcNow.AddDays(-days);

                // Note: we ignore QA1 returns, because these are not returned to the provider
                var returnedActivityIds = 
                    db.ActivityPqaQueue // PQA returns
                        .Where(e => e.IsCompleted && e.IsAccepted == false && e.LastModified >= cutoff)
                        .Select(e => e.ActivityId)
                    .Union(db.ActivityQa2Queue // QA2 returns
                        .Where(e => e.IsCompleted && e.IsAccepted == false && e.LastModified >= cutoff)
                        .Select(e => e.ActivityId))
                    .Union(db.ActivityEscalationQueue // Escalation returns
                        .Where(e => e.IsCompleted && e.IsAccepted == false && e.LastModified >= cutoff)
                        .Select(e => e.ActivityId));

                activities = activities.Where(a => returnedActivityIds.Contains(a.Id));
            }

            if (request.ApprovedWithinDays is { } approvedDays)
            {
                var approvedCutoff = DateTime.UtcNow.AddDays(-approvedDays);

                activities = activities.Where(a => a.Status == ActivityStatus.ApprovedStatus.Value
                                                   && a.CompletedOn >= approvedCutoff);
            }

#pragma warning disable CS8602
            var query = from a in activities
                        select new ActivityPaginationDto
                        {
                            ParticipantId = a.Participant.Id,
                            Participant = $"{a.Participant.FirstName} {a.Participant.LastName}",
                            Status = a.Status,
                            Definition = a.Definition,
                            ApprovedOn = a.CompletedOn,
                            LastModified = a.LastModified,
                            Created = a.Created,
                            CommencedOn = a.CommencedOn,
                            TookPlaceAtLocationName = a.TookPlaceAtLocation.Name,
                            AdditionalInformation = a.AdditionalInformation!,
                            ActivityId = a.Id,
                            SubmittedBy = $"{a.Owner.DisplayName!} ({a.Owner.TenantName})",
                            DocumentId = db.EducationTrainingActivities
                                .Where(e => e.Id == a.Id && e.DocumentId != Guid.Empty)
                                .Select(e => (Guid?)e.DocumentId)
                                .Concat(db.EmploymentActivities
                                    .Where(e => e.Id == a.Id && e.DocumentId != Guid.Empty)
                                    .Select(e => (Guid?)e.DocumentId))
                                .Concat(db.ISWActivities
                                    .Where(e => e.Id == a.Id && e.DocumentId != Guid.Empty)
                                    .Select(e => (Guid?)e.DocumentId))
                                .FirstOrDefault()
                        };
#pragma warning restore CS8602

            var count = await query.CountAsync(cancellationToken);

            var sortColumn = request.OrderBy.Trim().ToLowerInvariant() switch
            {
                "created" => "Created",
                "commencedon" => "CommencedOn",
                "status" => "Status",
                "lastmodified" => "LastModified",
                "approved" => "ApprovedOn",
                "location" => "TookPlaceAtLocationName",
                _ => "Created"
            };

            var sortDirection = request.SortDirection.Equals("Ascending", StringComparison.OrdinalIgnoreCase)
                ? "ascending"
                : "descending";

            var results = await query
                .OrderBy($"{sortColumn} {sortDirection}")
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedData<ActivityPaginationDto>(results, count, request.PageNumber, request.PageSize);
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(x => x.UserProfile.UserId)
                    .NotNull();

                RuleFor(x => x.SortDirection)
                    .Matches(ValidationConstants.SortDirection)
                    .WithMessage(ValidationConstants.SortDirectionMessage);

                RuleFor(x => x.OrderBy)
                    .Matches(ValidationConstants.AlphaNumeric)
                    .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "OrderBy"));
            }
        }
    }
}
