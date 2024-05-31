using Cfo.Cats.Application.Features.AuditTrails.DTOs;

namespace Cfo.Cats.Application.Features.AuditTrails.Queries.PaginationQuery;

public class AuditTrailsQueryHandler
    : IRequestHandler<AuditTrailsWithPaginationQuery, PaginatedData<AuditTrailDto>>
{
    private readonly IApplicationDbContext context;
    private readonly ICurrentUserService currentUserService;
    private readonly IMapper mapper;

    public AuditTrailsQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext context,
        IMapper mapper
    )
    {
        this.currentUserService = currentUserService;
        this.context = context;
        this.mapper = mapper;
    }

    public async Task<PaginatedData<AuditTrailDto>> Handle(
        AuditTrailsWithPaginationQuery request,
        CancellationToken cancellationToken
    )
    {
        var data = await context
            .AuditTrails.OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectToPaginatedDataAsync<AuditTrail, AuditTrailDto>(
                request.Specification,
                request.PageNumber,
                request.PageSize,
                mapper.ConfigurationProvider,
                cancellationToken
            );

        return data;
    }
}