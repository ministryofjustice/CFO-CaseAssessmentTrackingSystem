using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.AuditTrails.DTOs;

namespace Cfo.Cats.Application.Features.AuditTrails.Queries.GetSystemAuditTrailsWithPagination;

public class AuditTrailsQueryHandler
    : IRequestHandler<AuditTrailsWithPaginationQuery, PaginatedData<AuditTrailDto>>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ICurrentUserService currentUserService;
    private readonly IMapper mapper;

    public AuditTrailsQueryHandler(
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        IMapper mapper
    )
    {
        this.currentUserService = currentUserService;
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
    }

    public async Task<PaginatedData<AuditTrailDto>> Handle(
        AuditTrailsWithPaginationQuery request,
        CancellationToken cancellationToken
    )
    {
        var data = await unitOfWork.DbContext
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
