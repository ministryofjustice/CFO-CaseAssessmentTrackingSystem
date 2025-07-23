
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Application.Features.Tenants.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Tenants.Queries;

public static class GetHierarchy
{
    [RequestAuthorize(Policy = SecurityPolicies.SystemFunctionsRead)]
    public class Query : IRequest<Result<TenantHierarchyDto>>
    {
        public required UserProfile UserProfile { get; set; }
    }

    class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<TenantHierarchyDto>>
    {
        public async Task<Result<TenantHierarchyDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query =
                from tenant in unitOfWork.DbContext.Tenants
                where tenant.Id.StartsWith(request.UserProfile.TenantId!)
                orderby tenant.Id
                select new TenantHierarchyDto
                {
                    Id = tenant.Id,
                    Description = tenant.Name,
                    ParentId = tenant.Id.Substring(0, tenant.Id.Length - 2),
                    Depth = tenant.Id.Length / 2,
                    Users = (from user in unitOfWork.DbContext.Users
                            where user.TenantId == tenant.Id
                            orderby user.DisplayName
                            select new UserSummaryDto(user.Id, user.DisplayName, user.UserName)).ToList()
                };

            var tenants = await query
                .ToListAsync(cancellationToken);

            if(tenants is not { Count: > 0 })
            {
                return Result<TenantHierarchyDto>.Failure("No tenants were found.");
            }

            var parent = tenants.First();

            parent.Children = ToHierarchy(tenants, parent.Id);

            return Result<TenantHierarchyDto>.Success(parent);
        }

        static IEnumerable<TenantHierarchyDto> ToHierarchy(IEnumerable<TenantHierarchyDto> tenants, string tenantId)
        {
            var children = tenants.Where(t => t.ParentId == tenantId);

            foreach(var child in children)
            {
                child.Children = ToHierarchy(tenants, child.Id);
            }

            return children;
        }

    }
}
