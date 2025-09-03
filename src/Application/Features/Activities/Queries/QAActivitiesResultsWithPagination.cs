using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.Features.Activities.Specifications;
using Cfo.Cats.Application.SecurityConstants;
using static Cfo.Cats.Application.Features.Activities.DTOs.QAActivitiesResultsSummaryDto;

namespace Cfo.Cats.Application.Features.Activities.Queries;

public static class QAActivitiesResultsWithPagination
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Query : QAActivitiesResultsAdvancedFilter, IRequest<PaginatedData<QAActivitiesResultsSummaryDto>>
    {
        public QAActivitiesResultsAdvancedSpecification Specification => new(this);
    }

    public class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, PaginatedData<QAActivitiesResultsSummaryDto>>
    {
        public async Task<PaginatedData<QAActivitiesResultsSummaryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var db = unitOfWork.DbContext;

            bool hideUser = ShouldHideUser(request.CurentActiveUser);
            var CFOTenantNames = new HashSet<string> { "CFO", "CFO Evolution" };
            var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);

            var qaDefinitionIds = ActivityDefinition
                .List
                .Where(ad => ad.CheckType == CheckType.QA)
                .Select(ad => ad.Value)
                .ToList();
           
            var query = from a in db.Activities.AsNoTracking()
                        join p in db.Participants.AsNoTracking() on a.ParticipantId equals p.Id
                        where a.OwnerId == request.CurentActiveUser.UserId
                        && (
                            a.Status == ActivityStatus.PendingStatus.Value 
                            || (
                                a.Status == ActivityStatus.ApprovedStatus.Value &&
                                (
                                    (!request.CommencedStart.HasValue && !request.CommencedEnd.HasValue && a.ApprovedOn >= oneMonthAgo)
                                    || (
                                          (!request.CommencedStart.HasValue || a.CommencedOn >= request.CommencedStart) &&
                                          (!request.CommencedEnd.HasValue || a.CommencedOn <= request.CommencedEnd)
                                       )
                                    )
                                )
                            )

                        select new QAActivitiesResultsSummaryDto
                        {
                            ParticipantId = p.Id,
                            Participant = $"{p.FirstName} {p.LastName}",
                            Status = a.Status,
                            Definition = a.Definition,
                            ApprovedOn = a.ApprovedOn,
                            LastModified = a.LastModified,
                            Created = a.Created,
                            CommencedOn = a.CommencedOn,
                            TookPlaceAtLocationName = a.TookPlaceAtLocation.Name,
                            AdditionalInformation = a.AdditionalInformation!,
                            PQA = (from pqa in db.ActivityPqaQueue.AsNoTracking()
                                   from n in pqa.Notes
                                   join t in db.Tenants.AsNoTracking() on n.CreatedByUser!.TenantId equals t.Id
                                   where pqa.ActivityId == a.Id 
                                   select new ActSummaryNote(
                                       n.Message,
                                       CFOTenantNames.Contains(n.TenantId) && hideUser 
                                            ? "Hidden" 
                                            : n.CreatedByUser!.DisplayName!,
                                       t.Id, 
                                       t.Name!, 
                                       n.Created!.Value
                                   )).ToArray(),
                                                                        
                            QA1 = (from qa1 in db.ActivityQa1Queue 
                                   from n in qa1.Notes
                                   join t in db.Tenants on n.CreatedByUser!.TenantId equals t.Id
                                   where qa1.ActivityId == a.Id
                                   select new ActSummaryNote(
                                        n.Message,
                                        CFOTenantNames.Contains(n.TenantId) && hideUser
                                             ? "Hidden"
                                             : n.CreatedByUser!.DisplayName!,
                                        t.Id,
                                        t.Name!,
                                        n.Created!.Value
                                    )).ToArray(),

                            QA2 = (from qa2 in db.ActivityQa2Queue 
                                   from n in qa2.Notes
                                   join t in db.Tenants on n.CreatedByUser!.TenantId equals t.Id
                                   where qa2.ActivityId == a.Id
                                   select new ActSummaryNote(
                                         n.Message,
                                         CFOTenantNames.Contains(n.TenantId) && hideUser
                                              ? "Hidden"
                                              : n.CreatedByUser!.DisplayName!,
                                         t.Id,
                                         t.Name!,
                                         n.Created!.Value
                                    )).ToArray(),

                            Escalations = (from esc in db.ActivityEscalationQueue 
                                           from n in esc.Notes
                                           join t in db.Tenants on n.CreatedByUser!.TenantId equals t.Id
                                           where esc.ActivityId == a.Id
                                           select new ActSummaryNote(
                                               n.Message,
                                               CFOTenantNames.Contains(n.TenantId) && hideUser
                                                    ? "Hidden"
                                                    : n.CreatedByUser!.DisplayName!,
                                               t.Id,
                                               t.Name!,
                                               n.Created!.Value
                                          )).ToArray(),

                            Expiry = a.Expiry,
                            ActivityId = a.Id
                        };

            var ordered = query
              .OrderByDescending(a => a.Status == ActivityStatus.PendingStatus.Value)
              .ThenByDescending(a => a.ApprovedOn)
              .ThenBy(a => a.LastModified)
              .OrderBy($"{request.OrderBy} {request.SortDirection}");

            var list = await ordered.ToListAsync(cancellationToken);

            var qaActivities = list.Where(a => a.Definition.CheckType == CheckType.QA).ToList();

            var count = qaActivities.Count;
            var results = qaActivities
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

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
                RuleFor(x => x.CurentActiveUser.UserId)
                    .NotNull();
            }
        }
    }
}