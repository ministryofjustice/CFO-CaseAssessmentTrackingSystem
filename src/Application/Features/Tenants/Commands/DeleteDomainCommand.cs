using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Application.Features.Tenants.Commands;

[RequestAuthorize(Roles = "Admin")]
public static class DeleteDomainCommand
{
    [RequestAuthorize(Policy = PolicyNames.SystemFunctionsWrite)]
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
                CreateMap<Command, TenantDomain>();
            }
        }
    }

    internal class Handler(IApplicationDbContext context, IMapper mapper, ITenantService tenantService) : IRequestHandler<Command, Result<int>>
    {
        public async Task<Result<int>> Handle(Command request, CancellationToken cancellationToken)
        {
            var tenant = await context.Tenants.FindAsync(request.TenantId);

            if (tenant is null)
            {
                throw new NotFoundException(nameof(Tenant), request.TenantId!);
            }

            var model = mapper.Map<TenantDomain>(request);

            tenant.RemoveDomain(model);

            var result = await context.SaveChangesAsync(cancellationToken);

            tenantService.Refresh();

            return await Result<int>.SuccessAsync(result);
        }
    }

}