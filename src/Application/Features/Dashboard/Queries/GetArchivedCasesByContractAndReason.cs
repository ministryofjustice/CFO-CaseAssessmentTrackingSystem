using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetArchivedCasesByContractAndReason
{
    [RequestAuthorize(Policy = SecurityPolicies.Internal)]
    public class Query : IRequest<Result<ArchivedCasesByContractAndReasonDto>>
    {
        public string? TenantId { get; set; }
        public required UserProfile CurrentUser { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public string? UserId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<ArchivedCasesByContractAndReasonDto>>
    {
        public async Task<Result<ArchivedCasesByContractAndReasonDto>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var query =
                from ac in context.ArchivedCases.AsNoTracking()
                join c in context.Contracts on ac.ContractId equals c.Id
                join u in context.Users on c.CreatedBy equals u.Id
                where ac.From <= request.EndDate
                      && (ac.To == null || ac.To >= request.StartDate)
                      && c.Tenant!.Id.StartsWith(request.CurrentUser.TenantId!)
                select new { ac, c,u };

            if (!string.IsNullOrWhiteSpace(request.TenantId))
            {
                query = query.Where(x => x.c.Tenant!.Id.StartsWith(request.TenantId));
            }
            
            var tabularData = await query
                .OrderBy(x => x.ac.From)
                .Select(x => new ArchivedCasesTabularData
                {
                    ParticipantId = x.ac.ParticipantId,
                    FirstName = x.ac.FirstName,
                    LastName = x.ac.LastName,

                    ContractId = x.ac.ContractId,
                    Contract = x.c.Description,   // ✅ SAME AS ACTIVITIES

                    Reason = x.ac.ArchiveReason ?? "Unknown",

                    LocationId = x.ac.LocationId,

                    Created = x.ac.Created,
                    From = x.ac.From,
                    To = x.ac.To,

                    TenantId = x.ac.TenantId,
                    CreatedBy = x.u.DisplayName
                })
                .ToArrayAsync(cancellationToken);

            return Result<ArchivedCasesByContractAndReasonDto>
                .Success(new ArchivedCasesByContractAndReasonDto(tabularData));
        }
    }
    
    public record ArchivedCasesByContractAndReasonDto
    {
        public ArchivedCasesByContractAndReasonDto(
            ArchivedCasesTabularData[] tabularData)
        {
            TabularData = tabularData;

            ChartData = tabularData
                .GroupBy(td => new { td.Contract, td.Reason })
                .OrderBy(g => g.Key.Contract)
                .ThenBy(g => g.Key.Reason)
                .Select(g => new ArchivedCasesChartData
                {
                    Contract = g.Key.Contract,
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

        public string ContractId { get; set; } = "";     
        public string Contract { get; set; } = "";       
        public string? Reason { get; set; }

        public int LocationId { get; set; }

        public DateTime Created { get; set; }
        public DateTime From { get; set; }
        public DateTime? To { get; set; }

        public string? TenantId { get; set; }
        public string? CreatedBy { get; set; }
    }
    
    public record ArchivedCasesChartData
    {
        public string? Contract { get; set; }
        public string? Reason { get; set; }
        public int Count { get; set; }
    }
}