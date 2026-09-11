using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

/// <summary>
/// Scopes the user picker on the Performance / Employment tab to support workers who own an
/// activity with an eligible employment payment in the tenant scope, mirroring <see cref="GetEmployments"/>.
/// </summary>
public static class GetEmploymentAssignees
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

            var context = unitOfWork.DbContext;

            var assignees = await (
                    from mi in context.EmploymentPayments
                    join ap in context.Activities on mi.ActivityId equals ap.Id
                    where mi.EligibleForPayment
                       && mi.TenantId.StartsWith(tenantId)
                       && mi.PaymentPeriod >= request.StartDate
                       && mi.PaymentPeriod <= request.EndDate
                       && ap.OwnerId != null
                       && ap.Owner != null
                    select new AssigneeDto
                    {
                        Id = ap.OwnerId!,
                        DisplayName = ap.Owner!.DisplayName ?? ap.Owner!.UserName ?? string.Empty
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
