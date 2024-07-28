using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Application.Features.Tenants.Commands;

[RequestAuthorize(Roles = "Admin")]
public static class DeleteDomainCommand
{
    [RequestAuthorize(Policy = SecurityPolicies.SystemFunctionsWrite)]
    public class Command : IRequest<Result<int>>
    {
        [Description("Tenant Id")]
        public string? TenantId { get; set; }

        [Description("Email Domain")]
        public string? Domain { get; set; }

        private class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Command, TenantDomain>(MemberList.None);
            }
        }
    }

    internal class Handler(IUnitOfWork unitOfWork, IMapper mapper, ITenantService tenantService) : IRequestHandler<Command, Result<int>>
    {
        public async Task<Result<int>> Handle(Command request, CancellationToken cancellationToken)
        {
            var tenant = await unitOfWork.DbContext.Tenants.FindAsync(request.TenantId);

            if (tenant is null)
            {
                throw new NotFoundException(nameof(Tenant), request.TenantId!);
            }

            var model = mapper.Map<TenantDomain>(request);

            tenant.RemoveDomain(model);


            tenantService.Refresh();

            return Result<int>.Success(1);
        }
    }

}