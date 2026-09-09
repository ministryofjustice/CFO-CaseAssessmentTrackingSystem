using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.QualityAssurance.Specifications;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.QualityAssurance.Queries;

public static class GetPqaAssignees
{
    [RequestAuthorize(Policy = SecurityPolicies.Pqa)]
    public class Query(UserProfile currentUser) : IQuery<Result<AssigneeDto[]>>
    {
        public UserProfile CurrentUser { get; } = currentUser;
        public string? Keyword { get; set; }
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
            // Never filter by SupportWorkerId ourselves - we want every support worker currently
            // present in the filtered queue, not just the one that happens to already be selected.
            var filter = new QueueEntryFilter
            {
                CurrentUser = request.CurrentUser,
                Keyword = request.Keyword,
                TenantId = request.TenantId
            };

            var assignees = await unitOfWork.DbContext.EnrolmentPqaQueue
                .ApplySpecification(new EnrolmentPqaQueueEntrySpecification(filter))
                .AsNoTracking()
                .Where(e => e.SupportWorkerId != null && e.SupportWorker != null)
                .Select(e => new AssigneeDto
                {
                    Id = e.SupportWorkerId!,
                    DisplayName = e.SupportWorker!.DisplayName ?? e.SupportWorker!.UserName ?? string.Empty
                })
                .Distinct()
                .OrderBy(a => a.DisplayName)
                .ToArrayAsync(cancellationToken);

            return Result<AssigneeDto[]>.Success(assignees);
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.CurrentUser)
                .NotNull();
        }
    }
}
