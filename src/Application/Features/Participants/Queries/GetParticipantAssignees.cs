using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.Queries.Extensions;
using Cfo.Cats.Application.Features.Participants.Specifications;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Labels;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetParticipantAssignees
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query(UserProfile currentUser) : IQuery<Result<AssigneeDto[]>>
    {
        public UserProfile CurrentUser { get; } = currentUser;
        public ParticipantListView ListView { get; set; } = ParticipantListView.Default;
        public string? Keyword { get; set; }
        public int[] Locations { get; set; } = [];
        public LabelId? Label { get; set; }
        public string? TenantId { get; set; }
        public DateTime? RiskDue { get; set; }
        public RecentParticipantFilter RecentAction { get; set; } = RecentParticipantFilter.All;
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
            var tenantId = request.CurrentUser.TenantId;

            if (string.IsNullOrEmpty(tenantId))
            {
                return Result<AssigneeDto[]>.Success([]);
            }

            var query = unitOfWork.DbContext.Participants
                .AsNoTracking()
                .Where(p => p.OwnerId != null 
                         && p.Owner != null 
                         && p.Owner.TenantId != null 
                         && p.Owner.TenantId.StartsWith(tenantId))
                .ApplyKeywordSearch(request.Keyword)
                .ApplyLocationFilter(request.Locations)
                .ApplyListViewFilter(request.ListView)
                .ApplyLabelFilter(request.Label, unitOfWork.DbContext)
                .ApplyRiskDueFilter(request.RiskDue)
                .ApplyRecentActionFilter(request.RecentAction, request.CurrentUser.UserId, unitOfWork.DbContext);

            if (!string.IsNullOrEmpty(request.TenantId))
            {
                query = query.Where(p => p.Owner!.TenantId!.StartsWith(request.TenantId));
            }

            var assignees = await query
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
