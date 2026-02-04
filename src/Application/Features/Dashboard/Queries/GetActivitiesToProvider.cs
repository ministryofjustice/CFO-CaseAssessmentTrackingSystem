using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetActivitiesToProvider
{
    [RequestAuthorize(Policy = SecurityPolicies.Internal)]
    public class Query : IRequest<Result<ActivitiesToProviderDto>>
    {
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public string? UserId { get; set; }
        public string? TenantId { get; set; }
        public required UserProfile CurrentUser { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<ActivitiesToProviderDto>>
    {
        
        public async Task<Result<ActivitiesToProviderDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var query = from pfa in context.ProviderFeedbackActivities.AsNoTracking()
                where pfa.FeedbackType == null
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

                select new ActivitiesTabularData
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
                    ReturnedDate = pfa.ActionDate,
                    Message = pfa.Message ?? ""
                };

            var result = await query.OrderBy(r => r.ReturnedDate)
                            .AsNoTracking()
                            .ToArrayAsync(cancellationToken);

            return new ActivitiesToProviderDto(result);

        }

    }

    public record ActivitiesToProviderDto
    {
        public ActivitiesToProviderDto(ActivitiesTabularData[] tabularData)
        {
            TabularData = tabularData;
            
            // Get all unique contracts first, sorted
            var allContracts = tabularData
                .Select(td => td.ContractName)
                .Distinct()
                .OrderBy(c => c)
                .ToList();
            
            // Get all unique activity types
            var allActivityTypes = tabularData
                .Select(td => td.ActivityType)
                .Where(at => at != null)
                .Distinct()
                .OrderBy(at => at!.Name)
                .ToList();
            
            // Group the actual data
            var grouped = tabularData
                .GroupBy(td => new { td.ContractName, td.ActivityType })
                .Select(g => new ActivitiesChartData
                {
                    ContractName = g.Key.ContractName,
                    ActivityType = g.Key.ActivityType,
                    EscalationQueue = g.Count(x => x.Queue == "Escalation"),
                    QA2Queue = g.Count(x => x.Queue == "QA2")
                })
                .ToList();
            
            // Ensure each ActivityType has entries for all contracts (with 0 if missing)
            var completeData = new List<ActivitiesChartData>();
            
            foreach (var activityType in allActivityTypes)
            {
                foreach (var contract in allContracts)
                {
                    var existing = grouped.FirstOrDefault(x => 
                        x.ActivityType == activityType && 
                        x.ContractName == contract);
                        
                    completeData.Add(existing ?? new ActivitiesChartData
                    {
                        ContractName = contract,
                        ActivityType = activityType,
                        EscalationQueue = 0,
                        QA2Queue = 0
                    });
                }
            }
            
            ChartData = completeData.ToArray();
        }
        public ActivitiesTabularData[] TabularData { get;}
        public ActivitiesChartData[] ChartData { get;}
    }

    public record ActivitiesTabularData
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
        public DateTime? ReturnedDate { get; set; }
        public string? Message { get; set; }
    }
    public record ActivitiesChartData
    {
        public string? ContractName { get; set; }
        public ActivityType? ActivityType { get; set; }
        public string? Queue { get; set; }
        public int Count { get; set; }
        public int EscalationQueue { get; set; }
        public int QA2Queue { get; set; }
    }

}