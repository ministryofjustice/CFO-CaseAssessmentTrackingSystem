using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Tenants.Caching;
using Cfo.Cats.Application.Features.Tenants.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Tenants.Commands.Rename;

public static class RenameTenant
{
    [RequestAuthorize(Policy = PolicyNames.SystemFunctionsWrite)]
    public class Command  : ICacheInvalidatorRequest<Result<string>>
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public string[] CacheKeys => [TenantCacheKey.GetAllCacheKey];
        public CancellationTokenSource? SharedExpiryTokenSource =>
            TenantCacheKey.SharedExpiryTokenSource();
        
        private class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<TenantDto, Command>(MemberList.None);
            }
        }
        
    }

    internal class Handler(IApplicationDbContext context) : IRequestHandler<Command, Result<string>>
    {
        public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
        {
            var tenant = await context.Tenants.FindAsync(request.Id);
            if (tenant == null)
            {
                throw new NotFoundException("Tenant", request.Id);
            }
            tenant.Rename(request.Name);
            await context.SaveChangesAsync(cancellationToken);
            return tenant.Id;
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(r => r.Id)
                .NotNull()
                .NotEmpty();

            RuleFor(r => r.Name)
                .NotNull()
                .NotEmpty();
        }
    }


}
