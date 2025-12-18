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

            var qa2Data = context.ActivityQa2Queue
                .AsNoTracking()
                .SelectMany(q => q.Notes, (q, n) => new 
                {
                    Queue = "QA2",
                    ActivityId = q.ActivityId,
                    ParticipantId = q.ParticipantId,
                    CfoUser = q.OwnerId,
                    IsAccepted = q.IsAccepted,
                    IsCompleted = q.IsCompleted,
                    Message = n.Message,
                    Escalated = q.IsEscalated, // bool? matches DTO
                    ReturnedDate = q.Created,
                    SupportWorkerId = q.SupportWorkerId,
                    TenantId = q.TenantId
                });

            var escData = context.ActivityEscalationQueue
                .AsNoTracking()
                .SelectMany(q => q.Notes, (q, n) => new 
                {
                    Queue = "Escalation",
                    ActivityId = q.ActivityId,
                    ParticipantId = q.ParticipantId,
                    CfoUser = q.CreatedBy,
                    IsAccepted = q.IsAccepted,
                    IsCompleted = q.IsCompleted,
                    Message = n.Message,
                    Escalated = false, 
                    ReturnedDate = q.Created,
                    SupportWorkerId = q.SupportWorkerId,
                    TenantId = q.TenantId
                });

            var combined = qa2Data.Select(x => new
            {
                x.Queue,
                x.ActivityId,
                x.ParticipantId,
                x.CfoUser,
                x.IsAccepted,
                x.IsCompleted,
                x.Message,
                x.Escalated,
                x.ReturnedDate,
                x.SupportWorkerId,
                x.TenantId
            })
            .Concat(escData.Select(x => new
            {
                x.Queue,
                x.ActivityId,
                x.ParticipantId,
                x.CfoUser,
                x.IsAccepted,
                x.IsCompleted,
                x.Message,
                x.Escalated,
                x.ReturnedDate,
                x.SupportWorkerId,
                x.TenantId
            }));
            
            if (!string.IsNullOrWhiteSpace(request.TenantId))
            {
                combined = combined.Where(r => r.TenantId!.StartsWith(request.TenantId));
            }

            var query =
                from data in combined
                join cfoUser in context.Users on data.CfoUser equals cfoUser.Id into cfoUserJoin
                from cfoUser in cfoUserJoin.DefaultIfEmpty()
                join sw in context.Users on data.SupportWorkerId equals sw.Id into swJoin
                from sw in swJoin.DefaultIfEmpty()
                join c in context.Contracts on data.TenantId equals c.Tenant!.Id
                join a in context.Activities on data.ActivityId equals a.Id
 
                from latestSubmission in context.ActivityPqaQueue
                    .Where(eh =>
                        eh.ActivityId == data.ActivityId &&
                        eh.LastModified < data.ReturnedDate)
                    .OrderByDescending(eh => eh.LastModified)
                    .Take(1)
                    .DefaultIfEmpty()   // left-apply semantics

                join submittedByUser in context.Users on latestSubmission.LastModifiedBy equals submittedByUser.Id into submittedByUserJoin
                from submittedByUser in submittedByUserJoin.DefaultIfEmpty()

                where data.ReturnedDate >= request.StartDate
                && data.ReturnedDate <= request.EndDate
                && data.IsCompleted == true
                && data.IsAccepted == false

                select new ActivitiesTabularData
                {
                    ContractName = c.Description,
                    ParticipantId = data.ParticipantId,
                    Queue = data.Queue,
                    ActivityType = a.Type,
                    SupportWorker = sw.DisplayName,
                    CfoUser = cfoUser.DisplayName,

                    SubmittedDate = latestSubmission != null
                        ? (DateTime?)latestSubmission.LastModified
                        : null,
                    PqaUser = submittedByUser != null
                        ? submittedByUser.DisplayName
                        : null,

                    ReturnedDate = data.ReturnedDate,
                    IsCompleted = data.IsCompleted,
                    IsAccepted = data.IsAccepted ? "Yes" : "No",
                    Escalated = data.Escalated == true ? "Yes"
                            : data.Escalated == false ? "No"
                            : "",
                    Message = (data.Message ?? "").Replace("\r", " ").Replace("\n", " ")
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
        public DateTime? SubmittedDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public bool IsCompleted { get; set; }
        public string? IsAccepted { get; set; }
        public string? Escalated { get; set; }
        public string? Message { get; set; }
    }
    public record ActivitiesChartData
    {
        public string? ContractName { get; set; }
        public ActivityType? ActivityType { get; set; }
        public string? Queue { get; set; }
        public int EscalationQueue { get; set; }
        public int QA2Queue { get; set; }
    }

}