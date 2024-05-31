using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Application.Features.Tenants.Commands.AddEdit;

public class AddEditTenantCommandHandler : IRequestHandler<AddEditTenantCommand, Result<string>>
{
    private readonly IApplicationDbContext context;
    private readonly IStringLocalizer<AddEditTenantCommandHandler> localizer;
    private readonly IMapper mapper;
    private readonly ITenantService tenantsService;

    public AddEditTenantCommandHandler(
        ITenantService tenantsService,
        IApplicationDbContext context,
        IStringLocalizer<AddEditTenantCommandHandler> localizer,
        IMapper mapper
    )
    {
        this.tenantsService = tenantsService;
        this.context = context;
        this.localizer = localizer;
        this.mapper = mapper;
    }

    public async Task<Result<string>> Handle(
        AddEditTenantCommand request,
        CancellationToken cancellationToken
    )
    {
        var item = await context.Tenants.FindAsync(new object[] { request.Id }, cancellationToken);
        if (item is null)
        {
            item = mapper.Map<Tenant>(request);
            await context.Tenants.AddAsync(item, cancellationToken);
        }
        else
        {
            item = mapper.Map(request, item);
        }

        await context.SaveChangesAsync(cancellationToken);
        tenantsService.Refresh();
        return await Result<string>.SuccessAsync(item.Id);
    }
}