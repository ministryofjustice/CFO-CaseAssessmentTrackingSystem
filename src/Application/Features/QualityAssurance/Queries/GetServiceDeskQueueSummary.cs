using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.QualityAssurance.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Microsoft.EntityFrameworkCore;

namespace Cfo.Cats.Application.Features.QualityAssurance.Queries;

public static class GetServiceDeskQueueSummary
{
    [RequestAuthorize(Roles = $"{RoleNames.QAOfficer}, {RoleNames.QAManager}, {RoleNames.QASupportManager}, {RoleNames.SMT}, {RoleNames.SystemSupport}")]
    public class Query : IQuery<Result<ServiceDeskQueueSummaryDto>>
    {
        public required UserProfile CurrentUser { get; init; }
        public bool ShowEnrolments { get; init; } = true;
        public bool ShowActivities { get; init; } = true;
    }

    public class Handler(IUnitOfWork unitOfWork) : IQueryHandler<Query, Result<ServiceDeskQueueSummaryDto>>
    {
        public async Task<Result<ServiceDeskQueueSummaryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var tenantId = request.CurrentUser.TenantId!;
            var summary = new ServiceDeskQueueSummaryDto();

            if (request.ShowEnrolments)
            {
                summary = summary with
                {
                    EnrolmentQa1Count = await unitOfWork.DbContext.EnrolmentQa1Queue
                        .AsNoTracking()
                        .CountAsync(x => x.TenantId.StartsWith(tenantId) && x.IsCompleted == false, cancellationToken),
                    EnrolmentQa2Count = await unitOfWork.DbContext.EnrolmentQa2Queue
                        .AsNoTracking()
                        .CountAsync(x => x.TenantId.StartsWith(tenantId) && x.IsCompleted == false, cancellationToken),
                    EnrolmentEscalationCount = await unitOfWork.DbContext.EnrolmentEscalationQueue
                        .AsNoTracking()
                        .CountAsync(x => x.TenantId.StartsWith(tenantId) && x.IsCompleted == false, cancellationToken)
                };
            }

            if (request.ShowActivities)
            {
                summary = summary with
                {
                    ActivityQa1Count = await unitOfWork.DbContext.ActivityQa1Queue
                        .AsNoTracking()
                        .CountAsync(x => x.TenantId.StartsWith(tenantId) && x.IsCompleted == false, cancellationToken),
                    ActivityQa2Count = await unitOfWork.DbContext.ActivityQa2Queue
                        .AsNoTracking()
                        .CountAsync(x => x.TenantId.StartsWith(tenantId) && x.IsCompleted == false, cancellationToken),
                    ActivityEscalationCount = await unitOfWork.DbContext.ActivityEscalationQueue
                        .AsNoTracking()
                        .CountAsync(x => x.TenantId.StartsWith(tenantId) && x.IsCompleted == false, cancellationToken)
                };
            }

            return Result<ServiceDeskQueueSummaryDto>.Success(summary);
        }
    }
}
