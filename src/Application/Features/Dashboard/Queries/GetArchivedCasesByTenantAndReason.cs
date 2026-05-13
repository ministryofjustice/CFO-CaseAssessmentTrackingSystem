using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetArchivedCasesByTenantAndReason
{
    [RequestAuthorize(Policy = SecurityPolicies.Internal)]
    public class Query : IRequest<Result<ArchivedCasesByTenantAndReasonDto>>
    {
        public string? TenantId { get; init; }
        public required UserProfile CurrentUser { get; init; }
        public required DateTime StartDate { get; init; }
        public required DateTime EndDate { get; init; }
        public string? UserId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<ArchivedCasesByTenantAndReasonDto>>
    {
        public async Task<Result<ArchivedCasesByTenantAndReasonDto>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var query =
                from ac in context.ArchivedCases.AsNoTracking()
                join u in context.Users on ac.CreatedBy equals u.Id 
                join l in context.Locations on ac.LocationId equals l.Id
                where ac.From >= request.StartDate.Date
                      && ac.From < request.EndDate.AddDays(1).Date
                      && u.TenantId!.StartsWith(request.CurrentUser.TenantId!)

                select new { ac, u, l };

            if (!string.IsNullOrWhiteSpace(request.TenantId))
            {
                query = query.Where(x => x.u.TenantId!.StartsWith(request.TenantId));
            }
            
            var tabularData = await query
                .OrderBy(x => x.ac.From)
                .Select(x => new ArchivedCasesTabularData
                {
                    ParticipantId = x.ac.ParticipantId,
                    FirstName = x.ac.FirstName,
                    LastName = x.ac.LastName,
                    Reason = x.ac.ArchiveReason ?? "Unknown",

                    Location = x.l.Name,

                    Created = x.ac.Created,
                    From = x.ac.From,
                    To = x.ac.To,

                    TenantId = x.ac.TenantId!,
                    Tenant = x.u.TenantName!,
                    CreatedBy = x.u.DisplayName!
                })
                .ToArrayAsync(cancellationToken);

            return Result<ArchivedCasesByTenantAndReasonDto>
                .Success(new ArchivedCasesByTenantAndReasonDto(tabularData));
        }
    }
    
    public record ArchivedCasesByTenantAndReasonDto
    {
        public ArchivedCasesByTenantAndReasonDto(
            ArchivedCasesTabularData[] tabularData)
        {
            TabularData = tabularData;  

            ChartData = tabularData
                .GroupBy(td => new { td.Tenant, td.Reason })
                .OrderBy(g => g.Key.Tenant)
                .ThenBy(g => g.Key.Reason)
                .Select(g => new ArchivedCasesChartData
                {
                    Tenant = g.Key.Tenant,
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
        public required string ParticipantId { get; init; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        public required string TenantId { get; set; }
        public required string Tenant { get; set;}

        public required string Reason { get; init; }

        public required string Location { get; init; }

        public required DateTime Created { get; init; }
        public required DateTime From { get; init; }
        public DateTime? To { get; init; }

        public required string CreatedBy { get; init; }
    }
    
    public record ArchivedCasesChartData
    {
        public required string Tenant { get; init; }
        public required string Reason { get; init; }
        public required int Count { get; init; }
    }
}