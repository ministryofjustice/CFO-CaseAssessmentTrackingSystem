using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetActivitiesAdvisoriesToProvider
{
    [RequestAuthorize(Policy = SecurityPolicies.ProviderFeedback)]
    public class Query : IQuery<Result<ActivitiesAdvisoriesToProviderDto>>
    {
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public string? UserId { get; set; }
        public string? TenantId { get; set; }
        public bool IncludeInternalData { get; set; } = true;
        public required UserProfile CurrentUser { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IQueryHandler<Query, Result<ActivitiesAdvisoriesToProviderDto>>
    {
        
        public async Task<Result<ActivitiesAdvisoriesToProviderDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;
            var includeInternalData = request.IncludeInternalData && request.CurrentUser.HasInternalRole();

            var query = from pfa in context.ProviderFeedbackActivities.AsNoTracking()
                where pfa.Message != null
                    && (pfa.FeedbackType == ((int)FeedbackType.Advisory) || pfa.FeedbackType == ((int)FeedbackType.AcceptedByException))
                    && pfa.ActionDate >= request.StartDate
                    && pfa.ActionDate < request.EndDate.AddDays(1)
                    && (!string.IsNullOrWhiteSpace(request.TenantId) ? pfa.TenantId.StartsWith(request.TenantId) : true)
                join cfoUser in context.Users on pfa.CfoUserId equals cfoUser.Id into cfoUserJoin
                from cfoUser in cfoUserJoin.DefaultIfEmpty()
                join sw in context.Users on pfa.SupportWorkerId equals sw.Id into swJoin
                from sw in swJoin.DefaultIfEmpty()
                join a in context.Activities on pfa.ActivityId equals a.Id.ToString() into activityJoin
                from a in activityJoin.DefaultIfEmpty()
                join submittedByUser in context.Users on pfa.ProviderQaUserId equals submittedByUser.Id into submittedByUserJoin
                from submittedByUser in submittedByUserJoin.DefaultIfEmpty()
                join con in context.Contracts on pfa.ContractId equals con.Id into contractJoin
                from con in contractJoin.DefaultIfEmpty()
                
                select new ActivitiesAdvisoriesTabularData
                {
                    ContractName = con.Description,
                    ParticipantId = pfa.ParticipantId,
                    Queue = includeInternalData ? pfa.Queue : null,
                    ActivityType = a.Type,
                    SupportWorker = sw.DisplayName,
                    CfoUser = includeInternalData ? cfoUser.DisplayName : null,
                    PqaSubmittedDate = includeInternalData ? (DateTime?)pfa.PqaSubmittedDate : null,
                    PqaUser = includeInternalData ? submittedByUser.DisplayName : null,
                    AdvisoryDate = pfa.ActionDate,
                    FeedbackType = pfa.FeedbackType,
                    Message = pfa.Message ?? ""
                };

            var result = await query.OrderBy(r => r.AdvisoryDate)
                            .AsNoTracking()
                            .ToArrayAsync(cancellationToken);

            return new ActivitiesAdvisoriesToProviderDto(result.ToList(), includeInternalData);

        }

    }

    public record ActivitiesAdvisoriesToProviderDto
    {
        public ActivitiesAdvisoriesToProviderDto(List<ActivitiesAdvisoriesTabularData> tabularData, bool includeInternalData = true)
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
                    Total = g.Count(),
                    EscalationQueue = includeInternalData ? g.Count(x => x.Queue == "Escalation") : 0,
                    QA2Queue = includeInternalData ? g.Count(x => x.Queue == "QA2") : 0
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
        public int? FeedbackType { get; set; }
        public string? Message { get; set; }
    }
    public record ActivitiesAdvisoriesChartData
    {
        public string? ContractName { get; set; }
        public ActivityType? ActivityType { get; set; }
        public int Total { get; set; }
        public int EscalationQueue { get; set; }
        public int QA2Queue { get; set; }
    }

}