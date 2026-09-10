using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

/// <summary>
/// Scopes the user picker on the Initiatives dashboard to support workers who own a participant
/// with an initiative objective, mirroring <see cref="GetInitiativeObjectivesDashboard"/>.
/// </summary>
public static class GetInitiativeObjectiveAssignees
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

            var context = unitOfWork.DbContext;

            var assignees = await (
                    from p in context.Participants
                    join pp in context.PathwayPlans on p.Id equals pp.ParticipantId
                    join u in context.Users on p.OwnerId equals u.Id
                    join o in context.Objectives on pp.Id equals o.PathwayPlanId
                    join io in context.InitiativeObjectives on o.Id equals io.ObjectiveId
                    where io.TenantId.StartsWith(tenantId)
                    select new AssigneeDto
                    {
                        Id = u.Id,
                        DisplayName = u.DisplayName ?? u.UserName ?? string.Empty
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
