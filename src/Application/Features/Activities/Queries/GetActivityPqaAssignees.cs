using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Activities.Specifications;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Activities.Queries;

public static class GetActivityPqaAssignees
{
    [RequestAuthorize(Policy = SecurityPolicies.Pqa)]
    public class Query(UserProfile currentUser) : IQuery<Result<AssigneeDto[]>>
    {
        public UserProfile CurrentUser { get; } = currentUser;
        public string? Keyword { get; set; }
        public string? TenantId { get; set; }
        public int? ActivityTypeId { get; set; }
    }

    public class AssigneeDto
    {
        public required string Id { get; set; }
        public required string DisplayName { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IQueryHandler<Query, Result<AssigneeDto[]>>
    {
        public async Task<Result<AssigneeDto[]>> Handle(Query request, CancellationToken cancellationToken)
        {
            // Never filter by SupportWorkerId ourselves - we want every support worker currently
            // present in the filtered queue, not just the one that happens to already be selected.
            var filter = new ActivityQueueEntryFilter
            {
                CurrentUser = request.CurrentUser,
                Keyword = request.Keyword,
                TenantId = request.TenantId,
                ActivityTypeId = request.ActivityTypeId
            };

            var query = unitOfWork.DbContext.ActivityPqaQueue
                .ApplySpecification(new ActivityPqaQueueEntrySpecification(filter))
                .AsNoTracking();

            if (request.ActivityTypeId.HasValue)
            {
                var activityType = ActivityType.FromValue(request.ActivityTypeId.Value);
                query = query.Where(x => x.Activity!.Type == activityType);
            }

            var assignees = await query
                .Where(e => e.Participant!.OwnerId != null && e.Participant.Owner != null)
                .Select(e => new AssigneeDto
                {
                    Id = e.Participant!.OwnerId!,
                    DisplayName = e.Participant.Owner!.DisplayName ?? e.Participant.Owner!.UserName ?? string.Empty
                })
                .Distinct()
                .OrderBy(a => a.DisplayName)
                .ToArrayAsync(cancellationToken);

            return Result<AssigneeDto[]>.Success(assignees);
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator() =>
            RuleFor(x => x.CurrentUser)
                .NotNull();
    }
}
