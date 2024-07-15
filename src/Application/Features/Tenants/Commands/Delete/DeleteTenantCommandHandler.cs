using Cfo.Cats.Application.Common.Interfaces.MultiTenant;

namespace Cfo.Cats.Application.Features.Tenants.Commands.Delete;

public class DeleteTenantCommandHandler : IRequestHandler<DeleteTenantCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStringLocalizer<DeleteTenantCommandHandler> localizer;
    private readonly IMapper mapper;
    private readonly ITenantService tenantsService;

    public DeleteTenantCommandHandler(
        ITenantService tenantsService,
        IUnitOfWork unitOfWork,
        IStringLocalizer<DeleteTenantCommandHandler> localizer,
        IMapper mapper
    )
    {
        this.tenantsService = tenantsService;
        _unitOfWork = unitOfWork;
        this.localizer = localizer;
        this.mapper = mapper;
    }

    public async Task<Result<int>> Handle(
        DeleteTenantCommand request,
        CancellationToken cancellationToken
    )
    {
        var items = await _unitOfWork.DbContext
            .Tenants.Where(x => request.Id.Contains(x.Id))
            .ToListAsync(cancellationToken);
        foreach (var item in items)
        {
            _unitOfWork.DbContext.Tenants.Remove(item);
        }

        tenantsService.Refresh();
        return await Result<int>.SuccessAsync(items.Count);
    }
}