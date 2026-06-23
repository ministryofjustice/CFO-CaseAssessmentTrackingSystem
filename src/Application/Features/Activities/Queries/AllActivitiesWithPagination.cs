using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.Features.Activities.Specifications;
using Cfo.Cats.Application.SecurityConstants;

using static Cfo.Cats.Application.Features.Activities.DTOs.QAActivitiesResultsSummaryDto;

namespace Cfo.Cats.Application.Features.Activities.Queries;

public static class AllActivitiesWithPagination
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : AllActivitiesAdvancedFilter, IQuery<Result<PaginatedData<QAActivitiesResultsSummaryDto>>>
    {
        public AllActivitiesAdvancedSpecification Specification => new(this);
    }

    public class Handler(IUnitOfWork unitOfWork)
        : IQueryHandler<Query, Result<PaginatedData<QAActivitiesResultsSummaryDto>>>
    {
        public async Task<Result<PaginatedData<QAActivitiesResultsSummaryDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var db = unitOfWork.DbContext;

            var hideUser = ShouldHideUser(request.UserProfile);
            var CFOTenantNames = new HashSet<string> { "CFO", "CFO Evolution" };

            var activities = db.Activities.ApplySpecification(request.Specification);

            if (request.ReturnedWithinDays is { } days)
            {
                var cutoff = DateTime.UtcNow.AddDays(-days);

                var returnedActivityIds = db.ActivityPqaQueue
                    .Where(e => e.IsCompleted && e.IsAccepted == false && e.LastModified >= cutoff)
                    .Select(e => e.ActivityId)
                    .Union(db.ActivityQa1Queue
                        .Where(e => e.IsCompleted && e.IsAccepted == false && e.LastModified >= cutoff)
                        .Select(e => e.ActivityId))
                    .Union(db.ActivityQa2Queue
                        .Where(e => e.IsCompleted && e.IsAccepted == false && e.LastModified >= cutoff)
                        .Select(e => e.ActivityId))
                    .Union(db.ActivityEscalationQueue
                        .Where(e => e.IsCompleted && e.IsAccepted == false && e.LastModified >= cutoff)
                        .Select(e => e.ActivityId));

                activities = activities.Where(a => returnedActivityIds.Contains(a.Id));
            }

#pragma warning disable CS8602
            var query = from a in activities
                        select new QAActivitiesResultsSummaryDto
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
                            PQA = (from pqa in db.ActivityPqaQueue
                                   from n in pqa.Notes
                                   where pqa.ActivityId == a.Id
                                   select new ActSummaryNote(
                                       n.Message,
                                       CFOTenantNames.Contains(n.CreatedByUser.TenantName!) && hideUser
                                            ? "Hidden"
                                            : n.CreatedByUser!.DisplayName!,
                                       n.CreatedByUser.TenantId!,
                                       n.CreatedByUser.TenantName!,
                                       n.Created!.Value
                                   )).ToArray(),

                            QA1 = (from qa1 in db.ActivityQa1Queue
                                   from n in qa1.Notes
                                   where qa1.ActivityId == a.Id && (n.IsExternal || request.IncludeInternalNotes)
                                   select new ActSummaryNote(
                                        n.Message,
                                        CFOTenantNames.Contains(n.CreatedByUser.TenantName!) && hideUser
                                             ? "Hidden"
                                             : n.CreatedByUser!.DisplayName!,
                                        n.CreatedByUser.TenantId!,
                                        n.CreatedByUser.TenantName!,
                                        n.Created!.Value
                                    )).ToArray(),

                            QA2 = (from qa2 in db.ActivityQa2Queue
                                   from n in qa2.Notes
                                   where qa2.ActivityId == a.Id && (n.IsExternal || request.IncludeInternalNotes)
                                   select new ActSummaryNote(
                                         n.Message,
                                         CFOTenantNames.Contains(n.CreatedByUser.TenantName!) && hideUser
                                              ? "Hidden"
                                              : n.CreatedByUser!.DisplayName!,
                                        n.CreatedByUser.TenantId!,
                                        n.CreatedByUser.TenantName!,
                                         n.Created!.Value
                                    )).ToArray(),

                            Escalations = (from esc in db.ActivityEscalationQueue
                                           from n in esc.Notes
                                           where esc.ActivityId == a.Id && (n.IsExternal || request.IncludeInternalNotes)
                                           select new ActSummaryNote(
                                               n.Message,
                                               CFOTenantNames.Contains(n.CreatedByUser.TenantName!) && hideUser
                                                    ? "Hidden"
                                                    : n.CreatedByUser!.DisplayName!,
                                               n.CreatedByUser.TenantId!,
                                               n.CreatedByUser.TenantName!,
                                               n.Created!.Value
                                          )).ToArray(),
                            ActivityId = a.Id,
                            SubmittedBy = $"{a.Owner.DisplayName!} ({a.Owner.TenantName})"
                        };
#pragma warning restore CS8602

            var count = await query.CountAsync(cancellationToken);

            var sortColumn = request.OrderBy.Trim().ToLowerInvariant() switch
            {
                "created" => "Created",
                "commencedon" => "CommencedOn",
                "status" => "Status",
                "lastmodified" => "LastModified",
                "participant" => "Participant",
                "submittedby" => "SubmittedBy",
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

            return new PaginatedData<QAActivitiesResultsSummaryDto>(results, count, request.PageNumber, request.PageSize);
        }

        private bool ShouldHideUser(UserProfile user)
        {
            string[] allowed =
            [
                RoleNames.QAOfficer,
                RoleNames.QASupportManager,
                RoleNames.QAManager,
                RoleNames.SMT,
                RoleNames.SystemSupport
            ];

            return !user.AssignedRoles.Any(r => allowed.Contains(r));
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
