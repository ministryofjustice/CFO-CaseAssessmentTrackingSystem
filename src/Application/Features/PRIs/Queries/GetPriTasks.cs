using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.PathwayPlans.DTOs;
using Cfo.Cats.Application.Features.PRIs.Queries.Extensions;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.PRIs.Queries;

public static class GetPriTasks
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IQuery<Result<ObjectiveTaskDto[]>>
    {
        public required Guid PriId { get; set; }

        /// <summary>
        /// The currently logged-in user, used to apply the same tenant restriction as the PRI list
        /// </summary>
        public required UserProfile CurrentUser { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IQueryHandler<Query, Result<ObjectiveTaskDto[]>>
    {
        public async Task<Result<ObjectiveTaskDto[]>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var tasks = await (
                    from pri in context.PRIs
                        .Where(p => p.Id == request.PriId)
                        .ApplyTenantFilter(request.CurrentUser)
                    join task in context.ObjectiveTasks on pri.ObjectiveId equals task.ObjectiveId
                    orderby task.Index
                    select new ObjectiveTaskDto
                    {
                        Created = task.Created!.Value,
                        CreatedBy = task.CreatedBy!,
                        Description = task.Description,
                        Due = task.Due,
                        Id = task.Id,
                        IsMandatory = task.IsMandatory,
                        Index = task.Index,
                        Completed = task.Completed,
                        CompletedBy = task.CompletedBy,
                        CompletedStatus = task.CompletedStatus,
                        Justification = task.Justification,
                        ObjectiveId = task.ObjectiveId
                    })
                .ToArrayAsync(cancellationToken);

            return Result<ObjectiveTaskDto[]>.Success(tasks);
        }
    }
}
