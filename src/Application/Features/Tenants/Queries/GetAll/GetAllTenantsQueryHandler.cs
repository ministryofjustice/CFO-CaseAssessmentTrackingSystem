using Cfo.Cats.Application.Features.Tenants.DTOs;

namespace Cfo.Cats.Application.Features.Tenants.Queries.GetAll;

public class GetAllTenantsQueryHandler : IRequestHandler<GetAllTenantsQuery, IEnumerable<TenantDto>>
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

    public async Task<IEnumerable<TenantDto>> Handle(
        GetAllTenantsQuery request,
        CancellationToken cancellationToken
    )
    {
        var data = await unitOfWork.DbContext
            .Tenants.OrderBy(x => x.Name)
            .ProjectTo<TenantDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return data;
    }
}