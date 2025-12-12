using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Activities;

namespace Cfo.Cats.Application.Features.Activities.Queries;

public static class GetActivityQaNotes
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Query : IRequest<Result<ActivityQaNoteDto[]>>
    {
        public Guid? ActivityId { get; set; }

        public bool IncludeInternalNotes { get; set; }

        public UserProfile? CurrentUser { get; set; }
    }

    public class Handler(
        IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<ActivityQaNoteDto[]>>
    {
            public async Task<Result<ActivityQaNoteDto[]>> Handle(Query request, CancellationToken cancellationToken)
            {
                var pqa = await GetPqaNotes(request.ActivityId!);
                var qa1 = await GetQa1Notes(request.ActivityId!, request.IncludeInternalNotes);
                var qa2 = await GetQa2Notes(request.ActivityId!, request.IncludeInternalNotes);
                var es = await GetEscalationNotes(request.ActivityId!, request.IncludeInternalNotes);

                var allNotes = pqa
                    .Union(qa1)
                    .Union(qa2)
                    .Union(es)
                    .ToArray();

                return Result<ActivityQaNoteDto[]>.Success(allNotes);     
        }
        
        private static ActivityQaNoteDto Map(ActivityQueueEntryNote x) =>
            new()
            {
                Created = x.Created ?? DateTime.MinValue,
                Message = x.Message,
                CreatedBy = x.CreatedByUser?.DisplayName,
                TenantName = x.CreatedByUser?.TenantName,
                IsExternal = x.IsExternal,
                IsExpanded = false
            };

        private async Task<ActivityQaNoteDto[]> GetPqaNotes(Guid? activityId)
        {
            var entities = await unitOfWork.DbContext.ActivityPqaQueue
                .AsNoTracking()
                .Where(c => c.ActivityId == activityId)
                .SelectMany(c => c.Notes)
                .Include(n => n.CreatedByUser)
                .ToListAsync();

            return entities.Select(Map).ToArray();
        }
        
        private async Task<ActivityQaNoteDto[]> GetQa1Notes(Guid? activityId, bool includeInternalNotes)
        {
            var entities = await unitOfWork.DbContext.ActivityQa1Queue
                .AsNoTracking()
                .Where(c => c.ActivityId == activityId)
                .SelectMany(c => c.Notes.Where(n => n.IsExternal || includeInternalNotes))
                .Include(n => n.CreatedByUser)
                .ToListAsync();

            return entities.Select(Map).ToArray();
        }

        private async Task<ActivityQaNoteDto[]> GetQa2Notes(Guid? activityId, bool includeInternalNotes)
        {
            var entities = await unitOfWork.DbContext.ActivityQa2Queue
                .AsNoTracking()
                .Where(c => c.ActivityId == activityId)
                .SelectMany(c => c.Notes.Where(n => n.IsExternal || includeInternalNotes))
                .Include(n => n.CreatedByUser)
                .ToListAsync();

            return entities.Select(Map).ToArray();
        }

        private async Task<ActivityQaNoteDto[]> GetEscalationNotes(Guid? activityId, bool includeInternalNotes)
        {
            var entities = await unitOfWork.DbContext.ActivityEscalationQueue
                .AsNoTracking()
                .Where(c => c.ActivityId == activityId)
                .SelectMany(c => c.Notes.Where(n => n.IsExternal || includeInternalNotes))
                .Include(n => n.CreatedByUser)
                .ToListAsync();

            return entities.Select(Map).ToArray();
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator() =>
            RuleFor(x => x.CurrentUser)
                .NotNull();
    }
}