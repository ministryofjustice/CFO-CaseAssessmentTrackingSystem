using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.Features.Activities.Specifications;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Activities;

namespace Cfo.Cats.Application.Features.Activities.Queries;

public static class QAActivitiesResultsWithPagination
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Query : QAActivitiesResultsAdvancedFilter, IRequest<PaginatedData<QAActivitiesResultsSummaryDto>>
    {
        public QAActivitiesResultsAdvancedSpecification Specification => new(this);
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper)
        : IRequestHandler<Query, PaginatedData<QAActivitiesResultsSummaryDto>>
    {
        public async Task<PaginatedData<QAActivitiesResultsSummaryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var data = await unitOfWork.DbContext.Activities
                .Include(a => a.TookPlaceAtLocation)
                .Include(a => a.Participant)
                .ToListAsync(cancellationToken);

            var filtered = data.Where(a => a.RequiresQa && (a.Status == ActivityStatus.PendingStatus || a.Status == ActivityStatus.ApprovedStatus));

            var filteredDto = filtered
                  .AsQueryable()
                  .OrderByDescending(a => a.Status == ActivityStatus.PendingStatus.Value)
                  .ThenBy($"{request.OrderBy} {request.SortDirection}")
                  .ProjectToPaginatedData<Activity, QAActivitiesResultsSummaryDto>(
                      request.Specification,
                      request.PageNumber,
                      request.PageSize,
                      mapper.ConfigurationProvider
                  );

            bool hideUser = ShouldHideUser(request.CurentActiveUser);
            var activityIds = filteredDto.Items.Select(a => a.Id).ToArray();
            var notesByActivity = await GetNotesForActivitiesAsync(activityIds, hideUser, cancellationToken);
            
            foreach (var activity in filteredDto.Items)
            {
                notesByActivity.TryGetValue(activity.Id, out var notes);
                activity.Notes = notes ?? Array.Empty<ActivityQaNoteDto>();
            }
         
            return filteredDto;
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

            IEnumerable<ActivityQaNoteDto> MapNotes<T>(IEnumerable<T> queues,
                                                        bool forceExternal = false) where T : class
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