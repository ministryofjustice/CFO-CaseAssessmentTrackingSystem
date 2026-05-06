using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetInitiativeObjectivesDashboard
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<InitiativeObjectiveRowDto[]>>
    {
        public string? UserId { get; init; }
        public string? TenantId { get; init; }
        public required UserProfile CurrentUser { get; init; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<InitiativeObjectiveRowDto[]>>
    {
        public async Task<Result<InitiativeObjectiveRowDto[]>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var baseQuery = from p in context.Participants
                            join pp in context.PathwayPlans on p.Id equals pp.ParticipantId
                            join u in context.Users on p.OwnerId equals u.Id
                            join o in context.Objectives on pp.Id equals o.PathwayPlanId
                            join io in context.InitiativeObjectives on o.Id equals io.ObjectiveId
                            join i in context.Initiatives on io.InitiativeId equals i.Id
                            where p.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value
                            select new { p, u, o, i };

            baseQuery = request switch
            {
                { UserId: var userId } when !string.IsNullOrWhiteSpace(userId)
                    => baseQuery.Where(x => x.u.Id == userId),

                { TenantId: var tenantId } when !string.IsNullOrWhiteSpace(tenantId)
                    => baseQuery.Where(x => x.u.TenantId!.StartsWith(tenantId)),

                _ => throw new ArgumentException("Invalid request: UserId or TenantId must be provided.")
            };

            var resultQuery = from data in baseQuery
                              select new InitiativeObjectiveRowDto
                              {
                                  ParticipantId = data.p.Id,
                                  ParticipantName = data.p.FirstName + " " + data.p.LastName,
                                  ObjectiveId = data.o.Id,
                                  ObjectiveDescription = data.o.Description,
                                  IsObjectiveCompleted = data.o.Completed != null,
                                  InitiativeId = data.i.Id,
                                  InitiativeCode = data.i.Code,
                                  InitiativeDescription = data.i.Description,
                                  TotalTasks = data.o.Tasks.Count(),
                                  CompletedTasks = data.o.Tasks.Count(t => t.Completed != null),
                                  ActivityCount = context.Activities.Count(a => a.ObjectiveId == data.o.Id)
                              };

            var results = await resultQuery
                .AsNoTracking()
                .OrderBy(r => r.ParticipantName)
                .ThenBy(r => r.InitiativeCode)
                .ToArrayAsync(cancellationToken);

            return results;
        }
    }

    public record InitiativeObjectiveRowDto
    {
        public required string ParticipantId { get; init; }
        public required string ParticipantName { get; init; }
        public required Guid ObjectiveId { get; init; }
        public required string ObjectiveDescription { get; init; }
        public bool IsObjectiveCompleted { get; init; }
        public required Guid InitiativeId { get; init; }
        public required string InitiativeCode { get; init; }
        public required string InitiativeDescription { get; init; }
        public int TotalTasks { get; init; }
        public int CompletedTasks { get; init; }
        public int ActivityCount { get; init; }

        public string TaskSummary => TotalTasks == 0 ? "No tasks" : $"{CompletedTasks}/{TotalTasks} complete";
    }
}
