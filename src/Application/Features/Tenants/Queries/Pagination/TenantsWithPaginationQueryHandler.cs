using Cfo.Cats.Application.Features.Tenants.DTOs;
using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Application.Features.Tenants.Queries.Pagination;

public class TenantsWithPaginationQueryHandler
    : IRequestHandler<TenantsWithPaginationQuery, PaginatedData<TenantDto>>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IStringLocalizer<TenantsWithPaginationQueryHandler> localizer;
    private readonly IMapper mapper;

    public TenantsWithPaginationQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IStringLocalizer<TenantsWithPaginationQueryHandler> localizer
    )
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
        this.localizer = localizer;
    }

    public async Task<PaginatedData<TenantDto>> Handle(
        TenantsWithPaginationQuery request,
        CancellationToken cancellationToken
    )
    {
        var data = await unitOfWork.DbContext
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