using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Activities;

namespace Cfo.Cats.Application.Features.Activities.Queries;

public static class GetActivityQaNotes
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IQuery<Result<ActivityQaNoteDto[]>>
    {
        public Guid? ActivityId { get; set; }

        public bool IncludeInternalNotes { get; set; }

        public UserProfile? CurrentUser { get; set; }
    }

    public class Handler(
        IUnitOfWork unitOfWork) : IQueryHandler<Query, Result<ActivityQaNoteDto[]>>
    {
        private static readonly HashSet<string> CfoTenantNames = ["CFO", "CFO Evolution"];

        public async Task<Result<ActivityQaNoteDto[]>> Handle(Query request, CancellationToken cancellationToken)
        {
            var hideUser = ShouldHideUser(request.CurrentUser!);

            var pqa = await GetPqaNotes(request.ActivityId!, hideUser);
            var qa1 = await GetQa1Notes(request.ActivityId!, request.IncludeInternalNotes, hideUser);
            var qa2 = await GetQa2Notes(request.ActivityId!, request.IncludeInternalNotes, hideUser);
            var es = await GetEscalationNotes(request.ActivityId!, request.IncludeInternalNotes, hideUser);

            var allNotes = pqa
                .Union(qa1)
                .Union(qa2)
                .Union(es)
                .ToArray();

            return Result<ActivityQaNoteDto[]>.Success(allNotes);
        }

        private static ActivityQaNoteDto Map(ActivityQueueEntryNote x, bool hideUser) =>
            new()
            {
                Created = x.Created ?? DateTime.MinValue,
                Message = x.Message,
                CreatedBy = CfoTenantNames.Contains(x.CreatedByUser?.TenantName ?? string.Empty) && hideUser
                    ? "Hidden"
                    : x.CreatedByUser?.DisplayName,
                TenantName = x.CreatedByUser?.TenantName,
                IsExternal = x.IsExternal,
                ReturnReason = x.ReturnReason,
                IsExpanded = false
            };

        private async Task<ActivityQaNoteDto[]> GetPqaNotes(Guid? activityId, bool hideUser)
        {
            var entities = await unitOfWork.DbContext.ActivityPqaQueue
                .AsNoTracking()
                .Where(c => c.ActivityId == activityId)
                .SelectMany(c => c.Notes)
                .Include(n => n.CreatedByUser)
                .ToListAsync();

            return entities.Select(x => Map(x, hideUser)).ToArray();
        }

        private async Task<ActivityQaNoteDto[]> GetQa1Notes(Guid? activityId, bool includeInternalNotes, bool hideUser)
        {
            var entities = await unitOfWork.DbContext.ActivityQa1Queue
                .AsNoTracking()
                .Where(c => c.ActivityId == activityId)
                .SelectMany(c => c.Notes.Where(n => n.IsExternal || includeInternalNotes))
                .Include(n => n.CreatedByUser)
                .ToListAsync();

            return entities.Select(x => Map(x, hideUser)).ToArray();
        }

        private async Task<ActivityQaNoteDto[]> GetQa2Notes(Guid? activityId, bool includeInternalNotes, bool hideUser)
        {
            var entities = await unitOfWork.DbContext.ActivityQa2Queue
                .AsNoTracking()
                .Where(c => c.ActivityId == activityId)
                .SelectMany(c => c.Notes.Where(n => n.IsExternal || includeInternalNotes))
                .Include(n => n.CreatedByUser)
                .ToListAsync();

            return entities.Select(x => Map(x, hideUser)).ToArray();
        }

        private async Task<ActivityQaNoteDto[]> GetEscalationNotes(Guid? activityId, bool includeInternalNotes, bool hideUser)
        {
            var entities = await unitOfWork.DbContext.ActivityEscalationQueue
                .AsNoTracking()
                .Where(c => c.ActivityId == activityId)
                .SelectMany(c => c.Notes.Where(n => n.IsExternal || includeInternalNotes))
                .Include(n => n.CreatedByUser)
                .ToListAsync();

            return entities.Select(x => Map(x, hideUser)).ToArray();
        }

        private static bool ShouldHideUser(UserProfile user)
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
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator() =>
            RuleFor(x => x.CurrentUser)
                .NotNull();
    }
}