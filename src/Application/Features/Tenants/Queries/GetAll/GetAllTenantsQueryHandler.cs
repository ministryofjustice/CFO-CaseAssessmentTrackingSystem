using Cfo.Cats.Application.Features.Tenants.DTOs;

namespace Cfo.Cats.Application.Features.Tenants.Queries.GetAll;

public class GetAllTenantsQueryHandler : IRequestHandler<GetAllTenantsQuery, IEnumerable<TenantDto>>
{
    private readonly IApplicationDbContext context;
    private readonly IStringLocalizer<GetAllTenantsQueryHandler> localizer;
    private readonly IMapper mapper;

    public GetAllTenantsQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IStringLocalizer<GetAllTenantsQueryHandler> localizer
    )
    {
        this.context = context;
        this.mapper = mapper;
        this.localizer = localizer;
    }

    public async Task<IEnumerable<TenantDto>> Handle(
        GetAllTenantsQuery request,
        CancellationToken cancellationToken
    )
    {
        var data = await context
            .Tenants.OrderBy(x => x.Name)
            .ProjectTo<TenantDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return data;
    }
}