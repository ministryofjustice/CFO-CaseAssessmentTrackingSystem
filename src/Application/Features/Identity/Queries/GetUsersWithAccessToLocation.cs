using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Identity.Queries;

public static class GetUsersWithAccessToLocation
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IQuery<Result<IEnumerable<ApplicationUserSummaryDto>>>
    {
        public required int LocationId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, ICurrentUserService userService) : IQueryHandler<Query, Result<IEnumerable<ApplicationUserSummaryDto>>>
    {
        public async Task<Result<IEnumerable<ApplicationUserSummaryDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var location = await unitOfWork.DbContext.Locations
                .Where(l => l.Id == request.LocationId)
                .Include(location => location.Tenants)
                .FirstOrDefaultAsync(cancellationToken);

            if(location is null)
            {
                return Result<IEnumerable<ApplicationUserSummaryDto>>.Failure();
            }

            // Filter based on current user id; only load the columns we need
            var candidates = await unitOfWork.DbContext.Users
                .Where(u => u.TenantId!.StartsWith(userService.TenantId!)
                            && u.Status == UserStatus.Active)
                .Select(u => new { u.Id, u.UserName, u.DisplayName, u.TenantId })
                .ToListAsync(cancellationToken);

            // Note: This query seems to be non-EF-translatable :(
            // This resolves an issue where someone higher in the multitenancy tree (who has access to a given location)
            // assigns it to someone else lower in the tree (who may not necessarily have access to that same location).
            IEnumerable<ApplicationUserSummaryDto> users = candidates
                .Where(user => location.Tenants
                    .Any(tenant => tenant.Id.StartsWith(user.TenantId!))
                )
                .OrderBy(user => user.DisplayName)
                .Select(user => new ApplicationUserSummaryDto
                {
                    Id = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    DisplayName = user.DisplayName ?? string.Empty
                })
                .ToList();

            return Result<IEnumerable<ApplicationUserSummaryDto>>.Success(users);
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(q => q.LocationId)
                .NotEmpty();

            RuleSet(ValidationConstants.RuleSet.Mediator, () =>
            {
                RuleFor(q => q.LocationId)
                    .MustAsync(Exist);
            });
        }

        private async Task<bool> Exist(int locationId, CancellationToken cancellationToken) =>
            await _unitOfWork.DbContext.Locations
                .SingleOrDefaultAsync(l => l.Id == locationId, cancellationToken) is not null;
    }
}