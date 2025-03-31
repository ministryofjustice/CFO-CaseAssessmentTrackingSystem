using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Export;

public static class ExportCaseWorkload 
{
    [RequestAuthorize(Policy = SecurityPolicies.UserHasAdditionalRoles)]
    public class Query : IRequest<byte[]>
    {
        public required UserProfile CurrentUser { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, IExcelService excelService) : IRequestHandler<Query, byte[]>
    {
        public async Task<byte[]> Handle(Query request, CancellationToken cancellationToken)
        {
            var tenantId = $"{request.CurrentUser!.TenantId!}%";


            var data = await unitOfWork.DbContext.Database
                .SqlQuery<CaseSummaryDto>(
                    $@"SELECT 
            u.UserName,
            u.TenantName,
            p.EnrolmentStatus as EnrolmentStatusId, 
            l.Name as LocationName,
            COUNT(*) as [Count]
        FROM 
            Participant.Participant as p
        INNER JOIN
            [Configuration].Location as l ON p.CurrentLocationId = l.Id
        INNER JOIN
            [Identity].[User] as u ON p.OwnerId = u.Id
        WHERE
            u.TenantId LIKE {tenantId}  
        GROUP BY 
            u.UserName,
            u.TenantName,
            p.EnrolmentStatus,
            l.Name"
                ).ToArrayAsync(cancellationToken);

            var results = await excelService.ExportAsync(data,
                new Dictionary<string, Func<CaseSummaryDto, object?>>
                {
                    { "Tenant", item => item.TenantName },
                    { "Location", item => item.LocationName },
                    { "Status", item => item.GetEnrolmentStatus() },
                    { "User", item => item.UserName },
                    { "Count", item => item.Count },
                }
            );
            return results;


        }
    }
}