using Cfo.Cats.Application.Common.Interfaces.MultiTenant;

namespace Cfo.Cats.Application.Features.Tenants.Commands.Delete;

public class DeleteTenantCommandHandler : IRequestHandler<DeleteTenantCommand, Result<int>>
{
    private readonly IApplicationDbContext context;
    private readonly IStringLocalizer<DeleteTenantCommandHandler> localizer;
    private readonly IMapper mapper;
    private readonly ITenantService tenantsService;

    public DeleteTenantCommandHandler(
        ITenantService tenantsService,
        IApplicationDbContext context,
        IStringLocalizer<DeleteTenantCommandHandler> localizer,
        IMapper mapper
    )
    {
        this.tenantsService = tenantsService;
        this.context = context;
        this.localizer = localizer;
        this.mapper = mapper;
    }

    public async Task<Result<int>> Handle(
        DeleteTenantCommand request,
        CancellationToken cancellationToken
    )
    {
        var items = await context
            .Tenants.Where(x => request.Id.Contains(x.Id))
            .ToListAsync(cancellationToken);
        foreach (var item in items)
        {
            context.Tenants.Remove(item);
        }

        var result = await context.SaveChangesAsync(cancellationToken);
        tenantsService.Refresh();
        return await Result<int>.SuccessAsync(result);
    }
}