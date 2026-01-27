using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetArchivedCasesByRegionAndReason
{
    [RequestAuthorize(Policy = SecurityPolicies.Internal)]
    public class Query : IRequest<Result<ArchivedCasesByRegionAndReasonDto>>
    {
        public string? TenantId { get; set; }
        public required UserProfile CurrentUser { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public string? UserId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<ArchivedCasesByRegionAndReasonDto>>
    {
        public async Task<Result<ArchivedCasesByRegionAndReasonDto>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var query = context.ArchivedCases
                .AsNoTracking()
                .Where(ac =>
                    ac.From <= request.EndDate &&
                    (ac.To == null || ac.To >= request.StartDate) &&
                    ac.TenantId.StartsWith(request.CurrentUser.TenantId!)
                )
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.TenantId))
            {
                query = query.Where(ac => ac.TenantId.StartsWith(request.TenantId));
            }
            
            var tabularData = await query
                .OrderBy(ac => ac.From)
                .Select(ac => new ArchivedCasesTabularData
                {
                    ParticipantId = ac.ParticipantId,
                    FirstName = ac.FirstName,
                    LastName = ac.LastName,

                    Region = ac.LocationType,
                    Reason = ac.ArchiveReason ?? "Unknown",

                    ContractId = ac.ContractId,
                    LocationId = ac.LocationId,

                    Created = ac.Created,
                    From = ac.From,
                    To = ac.To,

                    TenantId = ac.TenantId,
                    CreatedBy = ac.CreatedBy
                })
                .ToArrayAsync(cancellationToken);

            return new ArchivedCasesByRegionAndReasonDto(tabularData);
        }
    }
    
    public record ArchivedCasesByRegionAndReasonDto
    {
        public ArchivedCasesByRegionAndReasonDto(
            ArchivedCasesTabularData[] tabularData)
        {
            TabularData = tabularData;

            ChartData = tabularData
                .GroupBy(td => new { td.Region, td.Reason })
                .OrderBy(g => g.Key.Region)
                .ThenBy(g => g.Key.Reason)
                .Select(g => new ArchivedCasesChartData
                {
                    Region = g.Key.Region,
                    Reason = g.Key.Reason,
                    Count = g.Count()
                })
                .ToArray();

        }

        public ArchivedCasesTabularData[] TabularData { get; }
        public ArchivedCasesChartData[] ChartData { get; }
    }
    
    public record ArchivedCasesTabularData
    {
        public string? ParticipantId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? Region { get; set; }
        public string? Reason { get; set; }

        public string? ContractId { get; set; }
        public int LocationId { get; set; }

        public DateTime Created { get; set; }
        public DateTime From { get; set; }
        public DateTime? To { get; set; }

        public string? TenantId { get; set; }
        public string? CreatedBy { get; set; }
    }
    
    public record ArchivedCasesChartData
    {
        public string? Region { get; set; }
        public string? Reason { get; set; }
        public int Count { get; set; }
    }
}