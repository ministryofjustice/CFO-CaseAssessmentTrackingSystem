using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Activities.Queries.Extensions;
using Cfo.Cats.Application.Features.Activities.Specifications;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Activities.Queries;

public static class GetActivityAssignees
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query(UserProfile currentUser) : IQuery<Result<AssigneeDto[]>>
    {
        public UserProfile CurrentUser { get; } = currentUser;
        public string? Keyword { get; set; }
        public string? TenantId { get; set; }
        public int? LocationId { get; set; }
        public int? Status { get; set; }
        public int? TypeFilter { get; set; }
        public int? ReturnedWithinDays { get; set; }
        public int? ApprovedWithinDays { get; set; }
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
            // Reuse the same specification as AllActivitiesWithPagination, but never filter by
            // OwnerId ourselves - we want every assignee currently present in the filtered set,
            // not just the one that happens to already be selected.
            var filter = new AllActivitiesAdvancedFilter
            {
                UserProfile = request.CurrentUser,
                Keyword = request.Keyword,
                TenantId = request.TenantId,
                LocationId = request.LocationId,
                Status = request.Status,
                TypeFilter = request.TypeFilter,
                ReturnedWithinDays = request.ReturnedWithinDays,
                ApprovedWithinDays = request.ApprovedWithinDays
            };

            var assignees = await unitOfWork.DbContext.Activities
                .ApplySpecification(new AllActivitiesAdvancedSpecification(filter))
                .ApplyReturnedWithinDaysFilter(unitOfWork.DbContext, request.ReturnedWithinDays)
                .ApplyApprovedWithinDaysFilter(request.ApprovedWithinDays)
                .Where(a => a.OwnerId != null && a.Owner != null)
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
