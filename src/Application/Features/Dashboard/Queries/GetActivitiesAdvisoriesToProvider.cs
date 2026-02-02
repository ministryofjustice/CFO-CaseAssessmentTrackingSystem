using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetActivitiesAdvisoriesToProvider
{
    [RequestAuthorize(Policy = SecurityPolicies.Internal)]
    public class Query : IRequest<Result<ActivitiesAdvisoriesToProviderDto>>
    {
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public string? UserId { get; set; }
        public string? TenantId { get; set; }
        public required UserProfile CurrentUser { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<ActivitiesAdvisoriesToProviderDto>>
    {
        
        public async Task<Result<ActivitiesAdvisoriesToProviderDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var query = from pfa in context.ProviderFeedbackActivities.AsNoTracking()
                where pfa.Message != null
                    && (pfa.FeedbackType == 0 || pfa.FeedbackType == 1)
                    && pfa.ActionDate >= request.StartDate
                    && pfa.ActionDate <= request.EndDate 
                    && pfa.TenantId.Length > 6
                    && (!string.IsNullOrWhiteSpace(request.TenantId) ? pfa.TenantId.StartsWith(request.TenantId) : true)
                join cfoUser in context.Users on pfa.CfoUserId equals cfoUser.Id into cfoUserJoin
                from cfoUser in cfoUserJoin.DefaultIfEmpty()
                join sw in context.Users on pfa.SupportWorkerId equals sw.Id into swJoin
                from sw in swJoin.DefaultIfEmpty()
                join a in context.Activities on pfa.ActivityId equals a.Id.ToString() into activityJoin
                from a in activityJoin.DefaultIfEmpty()
                join submittedByUser in context.Users on pfa.ProviderQaUserId equals submittedByUser.Id into submittedByUserJoin
                from submittedByUser in submittedByUserJoin.DefaultIfEmpty()

                select new ActivitiesAdvisoriesTabularData
                {
                    ContractName =
                        (from con in context.Contracts
                        where pfa.TenantId.StartsWith(con.Tenant!.Id)
                        orderby con.Tenant!.Id.Length descending
                        select con.Description)
                        .FirstOrDefault(),
                    ParticipantId = pfa.ParticipantId,
                    Queue = pfa.Queue,
                    ActivityType = a.Type,
                    SupportWorker = sw.DisplayName,
                    CfoUser = cfoUser.DisplayName,
                    PqaSubmittedDate = (DateTime?)pfa.PqaSubmittedDate,
                    PqaUser = submittedByUser.DisplayName,
                    AdvisoryDate = pfa.ActionDate,
                    Message = pfa.Message ?? ""
                };

            var result = await query.OrderBy(r => r.AdvisoryDate)
                            .AsNoTracking()
                            .ToArrayAsync(cancellationToken);

            return new ActivitiesAdvisoriesToProviderDto(result.ToList());

        }

    }

    public record ActivitiesAdvisoriesToProviderDto
    {
        public ActivitiesAdvisoriesToProviderDto(List<ActivitiesAdvisoriesTabularData> tabularData)
        {
            TabularData = tabularData;
            
            ChartData = tabularData
                .GroupBy(td => new { td.ContractName, td.ActivityType })
                .OrderBy(g => g.Key.ContractName)
                .ThenBy(g => g.Key.ActivityType?.Name)
                .Select(g => new ActivitiesAdvisoriesChartData
                {
                    ContractName = g.Key.ContractName,
                    ActivityType = g.Key.ActivityType,
                    EscalationQueue = g.Count(x => x.Queue == "Escalation"),
                    QA2Queue = g.Count(x => x.Queue == "QA2")
                })
                .ToArray();
        }
        public List<ActivitiesAdvisoriesTabularData> TabularData { get;}
        public ActivitiesAdvisoriesChartData[] ChartData { get;}
    }

    public record ActivitiesAdvisoriesTabularData
    {
        public string? TenantId { get; set; }
        public string? ContractName { get; set; }
        public ActivityType? ActivityType { get; set; }
        public string? Queue { get; set; }
        public string? ActivityId { get; set; } 
        public string? ParticipantId { get; set; }
        public string? SupportWorkerId { get; set; }
        public string? SupportWorker { get; set; }
        public string? PqaUser { get; set; }
        public string? CfoUser { get; set; }
        public DateTime? PqaSubmittedDate { get; set; }
        public DateTime? AdvisoryDate { get; set; }
        public bool IsCompleted { get; set; }
        public string? IsAccepted { get; set; }
        public string? Escalated { get; set; }
        public string? Message { get; set; }
    }
    public record ActivitiesAdvisoriesChartData
    {
        public string? ContractName { get; set; }
        public ActivityType? ActivityType { get; set; }
        public int EscalationQueue { get; set; }
        public int QA2Queue { get; set; }
    }

}