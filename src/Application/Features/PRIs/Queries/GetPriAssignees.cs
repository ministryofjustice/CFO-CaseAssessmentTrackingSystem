using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.PRIs.Queries.Extensions;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.PRIs.Queries;

public static class GetPriAssignees
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query(UserProfile currentUser) : IQuery<Result<PriAssigneesDto>>
    {
        public UserProfile CurrentUser { get; } = currentUser;
        public string? Keyword { get; set; }
        public bool JustMyPris { get; set; }
        public bool IncludeIncoming { get; set; }
        public bool IncludeOutgoing { get; set; }
        public string? CustodySupportWorker { get; set; }
        public string? CommunitySupportWorker { get; set; }
        public int? ExpectedReleaseRegionId { get; set; }
        public bool? ActiveStatus { get; set; }
    }

    public class AssigneeDto
    {
        public required string Id { get; set; }
        public required string DisplayName { get; set; }
    }

    public class PriAssigneesDto
    {
        public AssigneeDto[] CustodyWorkers { get; set; } = [];
        public AssigneeDto[] CommunityWorkers { get; set; } = [];
    }

    public class Handler(IUnitOfWork unitOfWork) : IQueryHandler<Query, Result<PriAssigneesDto>>
    {
        public async Task<Result<PriAssigneesDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            // Base filters shared by both roles - everything except the worker-specific filters,
            // so picking a worker for one role narrows the candidates for the other role but never
            // collapses its own dropdown down to just the already-selected worker.
            var baseQuery = unitOfWork.DbContext.PRIs
                .Where(x => PriStatus.ActiveList.Contains(x.Status))
                .ApplyTenantFilter(request.CurrentUser)
                .ApplyJustMyPrisFilter(request.JustMyPris, request.CurrentUser.UserId)
                .ApplyKeywordSearch(request.Keyword)
                .ApplyRegionFilter(request.ExpectedReleaseRegionId)
                .ApplyActiveStatusFilter(request.ActiveStatus);

            var custodyWorkers = await baseQuery
                .ApplyUserFilter(request.IncludeOutgoing, request.IncludeIncoming, request.CurrentUser.UserId,
                    custodyWorkerFilter: null, request.CommunitySupportWorker)
                .ApplyCommunityWorkerFilter(request.CommunitySupportWorker)
                .Where(p => p.CreatedBy != null)
                .Join(unitOfWork.DbContext.Users,
                    p => p.CreatedBy,
                    u => u.Id,
                    (p, u) => new AssigneeDto
                    {
                        Id = u.Id,
                        DisplayName = u.DisplayName ?? u.UserName ?? string.Empty
                    })
                .Distinct()
                .OrderBy(a => a.DisplayName)
                .ToArrayAsync(cancellationToken);

            var communityWorkers = await baseQuery
                .ApplyUserFilter(request.IncludeOutgoing, request.IncludeIncoming, request.CurrentUser.UserId,
                    request.CustodySupportWorker, communityWorkerFilter: null)
                .ApplyCustodyWorkerFilter(request.CustodySupportWorker)
                .Where(p => p.AssignedTo != null && p.AssignedToUser != null)
                .Select(p => new AssigneeDto
                {
                    Id = p.AssignedTo!,
                    DisplayName = p.AssignedToUser!.DisplayName ?? p.AssignedToUser!.UserName ?? string.Empty
                })
                .Distinct()
                .OrderBy(a => a.DisplayName)
                .ToArrayAsync(cancellationToken);

            return Result<PriAssigneesDto>.Success(new PriAssigneesDto
            {
                CustodyWorkers = custodyWorkers,
                CommunityWorkers = communityWorkers
            });
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
