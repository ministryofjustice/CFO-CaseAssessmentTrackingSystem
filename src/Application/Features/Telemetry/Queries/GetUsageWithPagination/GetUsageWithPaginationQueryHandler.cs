using Cfo.Cats.Application.Features.Telemetry.DTOs;
using Cfo.Cats.Domain.Entities.ManagementInformation;

namespace Cfo.Cats.Application.Features.Telemetry.Queries.GetUsageWithPagination;

public class GetUsageWithPaginationQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IQueryHandler<GetUsageWithPaginationQuery, Result<PaginatedData<UsageEventDto>>>
{
    public async Task<Result<PaginatedData<UsageEventDto>>> Handle(
        GetUsageWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var data = await unitOfWork.DbContext.UsageEvents
            .OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectToPaginatedDataAsync<UsageEvent, UsageEventDto>(
                request.Specification,
                request.PageNumber,
                request.PageSize,
                mapper.ConfigurationProvider,
                cancellationToken);

        return Result<PaginatedData<UsageEventDto>>.Success(data);
    }
}
