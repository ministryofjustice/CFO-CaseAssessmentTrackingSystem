using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Application.Features.Tenants.Commands.AddEdit;

public class AddEditTenantCommandHandler(
    ITenantService tenantsService,
    IUnitOfWork unitOfWork,
    IStringLocalizer<AddEditTenantCommandHandler> localizer,
    IMapper mapper
)
    : IRequestHandler<AddEditTenantCommand, Result<string>>
{
    private readonly IStringLocalizer<AddEditTenantCommandHandler> localizer = localizer;

    public async Task<Result<string>> Handle(
        AddEditTenantCommand request,
        CancellationToken cancellationToken
    )
    {
        var item = await unitOfWork.DbContext.Tenants.FindAsync(new object[] { request.Id }, cancellationToken);
        if (item is null)
        {
            item = mapper.Map<Tenant>(request);
            await unitOfWork.DbContext.Tenants.AddAsync(item, cancellationToken);
        }
        else
        {
            item = mapper.Map(request, item);
        }

        
        tenantsService.Refresh();
        return Result<string>.Success(item.Id);
    }
}