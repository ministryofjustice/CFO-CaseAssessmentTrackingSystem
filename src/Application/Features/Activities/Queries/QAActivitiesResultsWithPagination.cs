using Cfo.Cats.Application.Common.Extensions;
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
            bool hideUser = ShouldHideUser(request.CurentActiveUser);
            var CFOTenantNames = new HashSet<string> { "CFO", "CFO Evolution" };
            var query = from a in unitOfWork.DbContext.Activities.AsNoTracking()
                        join p in unitOfWork.DbContext.Participants on a.ParticipantId equals p.Id
                        where new[] { ActivityStatus.PendingStatus.Value, 
                            ActivityStatus.ApprovedStatus.Value }.Contains(a.Status)                             
                        select
                        new QAActivitiesResultsSummaryDto
                        {
                            ParticipantId = p.Id,
                            Participant = $"{p.FirstName} {p.LastName}",
                            Status = a.Status,
                            Definition = a.Definition,
                            ApprovedOn = a.ApprovedOn,
                            LastModified =  a.LastModified,
                            Created = a.Created,
                            CommencedOn = a.CommencedOn,
                            TookPlaceAtLocationName = a.TookPlaceAtLocation.Name,
                            AdditionalInformation = a.AdditionalInformation!,
                            PQA = (from q in unitOfWork.DbContext.ActivityPqaQueue from n in q.Notes where q.ActivityId == a.Id select new ActSummaryNote(n.Message, n.CreatedByUser!.DisplayName!, n.TenantId, n.Created!.Value)).ToArray(),
                            QA1 = (from q in unitOfWork.DbContext.ActivityQa1Queue from n in q.Notes where q.ActivityId == a.Id select new ActSummaryNote(n.Message, n.CreatedByUser!.DisplayName!, n.TenantId, n.Created!.Value)).ToArray(),
                            QA2 = (from q in unitOfWork.DbContext.ActivityQa2Queue from n in q.Notes where q.ActivityId == a.Id select new ActSummaryNote(n.Message, n.CreatedByUser!.DisplayName!, n.TenantId, n.Created!.Value)).ToArray(),
                            Escalations = (from q in unitOfWork.DbContext.ActivityEscalationQueue from n in q.Notes where q.ActivityId == a.Id select new ActSummaryNote(n.Message, n.CreatedByUser!.DisplayName!, n.TenantId, n.Created!.Value)).ToArray(),
                            Expiry = a.Expiry,
                            ActivityId = a.Id
                        };
            
            //var activityIds = filteredDto.Items.Select(a => a.Id).ToArray();
            //var notesByActivity = await GetNotesForActivitiesAsync(activityIds, hideUser, cancellationToken);

            var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);

            //var filtered = query.Where(a => // a.RequiresQa &&
            //    (a.Status == ActivityStatus.PendingStatus ||
            //    (a.Status == ActivityStatus.ApprovedStatus &&
            //    (
            //        // If no commenced filter was applied, enforce last month restriction
            //        (!request.CommencedStart.HasValue
            //        && !request.CommencedEnd.HasValue
            //        && a.ApprovedOn >= oneMonthAgo) ||

            //        // If commenced filter was applied, just allow (since pre-filter already handled it)
            //        (request.CommencedStart.HasValue || request.CommencedEnd.HasValue)
            //))));

            var count = await query.CountAsync(cancellationToken);

            var results = await query
                .OrderByDescending(a => a.Status == ActivityStatus.PendingStatus.Value)
                .ThenByDescending(a => a.ApprovedOn)
                .ThenBy(a => a.LastModified)
                .ThenBy($"{request.OrderBy} {request.SortDirection}")
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedData<QAActivitiesResultsSummaryDto>(results, count, request.PageNumber, request.PageSize);
            //.ProjectToPaginatedData<Activity, QAActivitiesResultsSummaryDto>(
            //    request.Specification,
            //    request.PageNumber,
            //    request.PageSize,
            //    mapper.ConfigurationProvider
            //);

            //var data = await unitOfWork.DbContext.Activities
            //    .Include(a => a.TookPlaceAtLocation)
            //    .Include(a => a.Participant)
            //    .ToListAsync(cancellationToken);

            //var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);

            //var filtered = data.Where(a => a.RequiresQa &&
            //    (a.Status == ActivityStatus.PendingStatus ||
            //    (a.Status == ActivityStatus.ApprovedStatus &&
            //    (
            //        // If no commenced filter was applied, enforce last month restriction
            //        (!request.CommencedStart.HasValue
            //        && !request.CommencedEnd.HasValue
            //        && a.ApprovedOn >= oneMonthAgo) ||

            //        // If commenced filter was applied, just allow (since pre-filter already handled it)
            //        (request.CommencedStart.HasValue || request.CommencedEnd.HasValue)
            //))));

            //var filteredDto = filtered
            //      .AsQueryable()
            //      .OrderByDescending(a => a.Status == ActivityStatus.PendingStatus.Value)
            //      .ThenByDescending(a => a.ApprovedOn)
            //      .ThenBy(a => a.LastModified)
            //      .ThenBy($"{request.OrderBy} {request.SortDirection}")
            //      .ProjectToPaginatedData<Activity, QAActivitiesResultsSummaryDto>(
            //          request.Specification,
            //          request.PageNumber,
            //          request.PageSize,
            //          mapper.ConfigurationProvider
            //      );
            //bool hideUser = ShouldHideUser(request.CurentActiveUser);
            //var activityIds = filteredDto.Items.Select(a => a.Id).ToArray();
            //var notesByActivity = await GetNotesForActivitiesAsync(activityIds, hideUser, cancellationToken);

            //foreach (var activity in filteredDto.Items)
            //{
            //    notesByActivity.TryGetValue(activity.Id, out var notes);
            //    activity.Notes = notes ?? Array.Empty<ActivityQaNoteDto>();
            //}

            //return filteredDto;
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

        private async Task<Dictionary<Guid, ActivityQaNoteDto[]>> GetNotesForActivitiesAsync(Guid[] activityIds, bool hideUser,    CancellationToken cancellationToken)
        {
            var CFOTenantNames = new HashSet<string> { "CFO", "CFO Evolution" };

            // Fetch all queues in one go per type
            var pqaQueues = await unitOfWork.DbContext.ActivityPqaQueue
                .AsNoTracking()
                .Where(q => activityIds.Contains(q.ActivityId))
                .Include(q => q.Notes)
                .ThenInclude(n => n.CreatedByUser)
                .ToListAsync(cancellationToken);

            var qa1Queues = await unitOfWork.DbContext.ActivityQa1Queue
                .AsNoTracking()
                .Where(q => activityIds.Contains(q.ActivityId))
                .Include(q => q.Notes)
                .ThenInclude(n => n.CreatedByUser)
                .ToListAsync(cancellationToken);

            var qa2Queues = await unitOfWork.DbContext.ActivityQa2Queue
                .AsNoTracking()
                .Where(q => activityIds.Contains(q.ActivityId))
                .Include(q => q.Notes)
                .ThenInclude(n => n.CreatedByUser)
                .ToListAsync(cancellationToken);

            var esQueues = await unitOfWork.DbContext.ActivityEscalationQueue
                .AsNoTracking()
                .Where(q => activityIds.Contains(q.ActivityId))
                .Include(q => q.Notes)
                .ThenInclude(n => n.CreatedByUser)
                .ToListAsync(cancellationToken);

            IEnumerable<ActivityQaNoteDto> MapNotes<T>(IEnumerable<T> queues, bool forceExternal = false) where T : class
            {
                foreach (dynamic q in queues)
                {
                    foreach (var n in q.Notes)
                    {
                        if (forceExternal || n.IsExternal) 
                        {
                            yield return new ActivityQaNoteDto                    
                            {
                                ActivityId = q.ActivityId,
                                Created = n.Created,
                                Message = n.Message,
                                CreatedBy = CFOTenantNames.Contains(n.CreatedByUser.TenantName) && hideUser
                                    ? "CFO User"
                                    : n.CreatedByUser.DisplayName,
                                TenantName = n.CreatedByUser.TenantName,
                                IsExternal = forceExternal ? true : n.IsExternal
                            };
                        }
                    }
                }
            }

            // Map notes
            var allNotes = MapNotes(pqaQueues, forceExternal: true)   // PQA notes always external
                .Concat(MapNotes(qa1Queues))
                .Concat(MapNotes(qa2Queues))
                .Concat(MapNotes(esQueues))
                .OrderByDescending(n => n.Created)
                .ToList();

            // Group by activity ID
            return allNotes
                .GroupBy(n => n.ActivityId)
                .ToDictionary(g => g.Key, g => g.ToArray());
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