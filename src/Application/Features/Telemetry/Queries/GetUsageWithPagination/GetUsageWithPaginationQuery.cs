using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Telemetry.DTOs;
using Cfo.Cats.Application.Features.Telemetry.Specifications;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Telemetry.Queries.GetUsageWithPagination;

[RequestAuthorize(Policy = SecurityPolicies.ServiceDeskManagement)]
public class GetUsageWithPaginationQuery
    : UsageEventAdvancedFilter, IQuery<Result<PaginatedData<UsageEventDto>>>
{
    public UsageEventAdvancedSpecification Specification => new(this);

    public GetUsageWithPaginationQuery()
    {
        OrderBy = nameof(Domain.Entities.ManagementInformation.UsageEvent.OccurredOn);
    }

    public override string ToString()
        => $"ListView:{ListView},Area:{Area},Search:{Keyword},Sort:{SortDirection},OrderBy:{OrderBy},{PageNumber},{PageSize}";
}
