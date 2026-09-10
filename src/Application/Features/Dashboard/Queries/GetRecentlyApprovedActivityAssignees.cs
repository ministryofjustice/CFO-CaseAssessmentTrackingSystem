using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

/// <summary>
/// Scopes the user picker on the Recently Approved Activities dashboard to support workers who
/// own an approved activity within the tenant scope, mirroring <see cref="GetRecentlyApprovedActivities"/>.
/// </summary>
public static class GetRecentlyApprovedActivityAssignees
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query(UserProfile currentUser) : IQuery<Result<AssigneeDto[]>>
    {
        public UserProfile CurrentUser { get; } = currentUser;
        public string? TenantId { get; set; }
        public required DateTime StartDate { get; init; }
        public required DateTime EndDate { get; init; }
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
            var tenantId = request.TenantId ?? request.CurrentUser.TenantId;

            if (string.IsNullOrEmpty(tenantId))
            {
                return Result<AssigneeDto[]>.Success([]);
            }

            var startDate = request.StartDate.Date;
            var endDate = request.EndDate.Date.AddDays(1);

            var assignees = await unitOfWork.DbContext.Activities
                .AsNoTracking()
                .Where(a => a.Status == ActivityStatus.ApprovedStatus.Value
                         && a.CompletedOn >= startDate
                         && a.CompletedOn <= endDate
                         && a.OwnerId != null
                         && a.Owner != null
                         && a.TenantId.StartsWith(tenantId))
                .Select(a => new AssigneeDto
                {
                    Id = a.OwnerId!,
                    DisplayName = a.Owner!.DisplayName ?? a.Owner!.UserName ?? string.Empty
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
