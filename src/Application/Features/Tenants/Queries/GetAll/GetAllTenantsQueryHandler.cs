using Cfo.Cats.Application.Features.Tenants.DTOs;

namespace Cfo.Cats.Application.Features.Tenants.Queries.GetAll;

public class GetAllTenantsQueryHandler : IRequestHandler<GetAllTenantsQuery, Result<IEnumerable<TenantDto>>>
{
    private readonly IStringLocalizer<GetAllTenantsQueryHandler> localizer;
    private readonly IMapper mapper;
    private readonly IUnitOfWork unitOfWork;

    public GetAllTenantsQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IStringLocalizer<GetAllTenantsQueryHandler> localizer
    )
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
        this.localizer = localizer;
    }

    public async Task<Result<IEnumerable<TenantDto>>> Handle(
        GetAllTenantsQuery request,
        CancellationToken cancellationToken
    )
    {
        var data = await unitOfWork.DbContext.Tenants
            .Where(t => t.Id.StartsWith(request.UserProfile!.TenantId!))
            .OrderBy(x => x.Id)
            .ProjectTo<TenantDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        
        return Result<IEnumerable<TenantDto>>.Success(data);
        
    }
}