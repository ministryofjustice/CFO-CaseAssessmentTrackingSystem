using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetActivitiesInQaPots
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IQuery<Result<ActivitiesInQaPotsDto>>
    {
        public required UserProfile CurrentUser { get; set; }
        public required bool IncludeTeams { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IQueryHandler<Query, Result<ActivitiesInQaPotsDto>>
    {
        public async Task<Result<ActivitiesInQaPotsDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var dto = new ActivitiesInQaPotsDto();

            var activities = context.Activities.AsNoTracking();

            if (request.IncludeTeams)
            {
                var tenantId = request.CurrentUser.TenantId;

                if (string.IsNullOrEmpty(tenantId))
                {
                    return Result<ActivitiesInQaPotsDto>.Success(dto);
                }

                activities = activities.Where(a => a.TenantId.StartsWith(tenantId));
            }
            else
            {
                var userId = request.CurrentUser.UserId;
                activities = activities.Where(a => a.OwnerId == userId);
            }

            var pqa = await (
                    from q in context.ActivityPqaQueue.AsNoTracking()
                    join a in activities on q.ActivityId equals a.Id
                    where !q.IsCompleted
                    group q by a.Type
                    into grouped
                    select new
                    {
                        grouped.Key,
                        Count = grouped.Count()
                    }
                )
                .ToDictionaryAsync(x => x.Key, x => x.Count, cancellationToken);

            var qa1 =
                from q in context.ActivityQa1Queue.AsNoTracking()
                where !q.IsCompleted
                select q.ActivityId;

            var qa2 =
                from q in context.ActivityQa2Queue.AsNoTracking()
                where !q.IsCompleted
                select q.ActivityId;

            var escalation =
                from q in context.ActivityEscalationQueue.AsNoTracking()
                where !q.IsCompleted
                select q.ActivityId;

            var combined = qa1
                .Concat(qa2)
                .Concat(escalation);

            var cfo = await (
                    from activityId in combined
                    join a in activities on activityId equals a.Id
                    group activityId by a.Type
                    into grouped
                    select new
                    {
                        grouped.Key,
                        Count = grouped.Count()
                    }
                )
                .ToDictionaryAsync(x => x.Key, x => x.Count, cancellationToken);

            dto.EducationCfo = cfo.GetValueOrDefault(ActivityType.EducationAndTraining);
            dto.EmploymentCfo = cfo.GetValueOrDefault(ActivityType.Employment);
            dto.IswSupportCfo = cfo.GetValueOrDefault(ActivityType.InterventionsAndServicesWraparoundSupport);

            dto.EducationPqa = pqa.GetValueOrDefault(ActivityType.EducationAndTraining);
            dto.EmploymentPqa = pqa.GetValueOrDefault(ActivityType.Employment);
            dto.IswSupportPqa = pqa.GetValueOrDefault(ActivityType.InterventionsAndServicesWraparoundSupport);

            return Result<ActivitiesInQaPotsDto>.Success(dto);
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator() =>
            RuleFor(q => q.CurrentUser)
                .NotNull();
    }
}
