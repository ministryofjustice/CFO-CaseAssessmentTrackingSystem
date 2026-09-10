using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

/// <summary>
/// Scopes the user picker on the Location Breakdown dashboard to support workers who actually own
/// a (non-archived) participant within the tenant scope, mirroring <see cref="GetCasesPerLocation"/>.
/// </summary>
public static class GetCasesPerLocationAssignees
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query(UserProfile currentUser) : IQuery<Result<AssigneeDto[]>>
    {
        public UserProfile CurrentUser { get; } = currentUser;
        public string? TenantId { get; set; }
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

            var assignees = await unitOfWork.DbContext.Participants
                .AsNoTracking()
                .Where(p => p.OwnerId != null
                         && p.Owner != null
                         && p.Owner.TenantId != null
                         && p.Owner.TenantId.StartsWith(tenantId)
                         && p.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value)
                .Select(p => new AssigneeDto
                {
                    Id = p.OwnerId!,
                    DisplayName = p.Owner!.DisplayName ?? p.Owner!.UserName ?? string.Empty
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
