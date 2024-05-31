using Cfo.Cats.Application.Features.Tenants.DTOs;
using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Application.Features.Tenants.Queries.Pagination;

public class TenantsWithPaginationQueryHandler
    : IRequestHandler<TenantsWithPaginationQuery, PaginatedData<TenantDto>>
{
    private readonly IApplicationDbContext context;
    private readonly IStringLocalizer<TenantsWithPaginationQueryHandler> localizer;
    private readonly IMapper mapper;

    public TenantsWithPaginationQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IStringLocalizer<TenantsWithPaginationQueryHandler> localizer
    )
    {
        this.context = context;
        this.mapper = mapper;
        this.localizer = localizer;
    }

    public async Task<PaginatedData<TenantDto>> Handle(
        TenantsWithPaginationQuery request,
        CancellationToken cancellationToken
    )
    {
        var data = await context
            .Tenants.OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectToPaginatedDataAsync<Tenant, TenantDto>(
                request.Specification,
                request.PageNumber,
                request.PageSize,
                mapper.ConfigurationProvider,
                cancellationToken
            );
        return data;
    }
}