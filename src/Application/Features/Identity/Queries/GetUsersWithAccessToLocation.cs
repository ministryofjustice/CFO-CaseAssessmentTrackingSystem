using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Identity.Queries;

public static class GetUsersWithAccessToLocation
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<IEnumerable<ApplicationUserDto>>>
    {
        public required int LocationId { get; set; }
    }

    private class Handler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService userService) : IRequestHandler<Query, Result<IEnumerable<ApplicationUserDto>>>
    {
        public async Task<Result<IEnumerable<ApplicationUserDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var location = await unitOfWork.DbContext.Locations
                .Where(l => l.Id == request.LocationId)
                .Include(location => location.Tenants)
                .FirstOrDefaultAsync(cancellationToken);

            if(location is null)
            {
                return Result<IEnumerable<ApplicationUserDto>>.Failure();
            }

            // Filter based on current user id
            var users = await unitOfWork.DbContext.Users
                .Where(u => u.TenantId!.StartsWith(userService.TenantId!))
                .ProjectTo<ApplicationUserDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            // Note: This query seems to be non-EF-translatable :(
            // This resolves an issue where someone higher in the multitenancy tree (who has access to a given location)
            // assigns it someone else lower in the tree (who may not neccessarily have access to that same location).
            users = users
                .Where(user => location.Tenants
                    .Any(tenant => tenant.Id.StartsWith(user.TenantId!))
                ).OrderBy(user => user.DisplayName)
                .ToList();

            return Result<IEnumerable<ApplicationUserDto>>.Success(users);
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        private readonly IUnitOfWork unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;

            RuleFor(q => q.LocationId)
                .NotEmpty();

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(q => q.LocationId)
                    .MustAsync(Exist);
            });
        }

        private async Task<bool> Exist(int locationId, CancellationToken cancellationToken)
        {
            return await unitOfWork.DbContext.Locations
                .SingleOrDefaultAsync(l => l.Id == locationId, cancellationToken) is not null;
        }
    }
}