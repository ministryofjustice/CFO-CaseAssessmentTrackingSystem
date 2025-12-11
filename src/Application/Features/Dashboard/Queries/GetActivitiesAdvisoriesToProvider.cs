using System.Diagnostics;
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
                    AdvisoryDate = q.Created,
                    SupportWorkerId = q.SupportWorkerId,
                    TenantId = q.TenantId
                })
                .Where(n => n.Message != null
                && (n.Message.StartsWith("Advisory") || n.Message.StartsWith("Accepted by Exception"))
                && n.IsAccepted == true
                && n.IsCompleted == true);

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
                    AdvisoryDate = q.Created,
                    SupportWorkerId = q.SupportWorkerId,
                    TenantId = q.TenantId
                })
                .Where(n => n.Message != null
                && (n.Message.StartsWith("Advisory") || n.Message.StartsWith("Accepted by Exception"))
                && n.IsAccepted == true
                && n.IsCompleted == true);

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
                x.AdvisoryDate,
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
                x.AdvisoryDate,
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
                join a in context.Activities on data.ActivityId equals a.Id
 
                from latestSubmission in context.ActivityPqaQueue
                    .Where(eh =>
                        eh.ActivityId == data.ActivityId &&
                        eh.LastModified < data.AdvisoryDate)
                    .OrderByDescending(eh => eh.LastModified)
                    .Take(1)
                    .DefaultIfEmpty()   // left-apply semantics

                join submittedByUser in context.Users on latestSubmission.LastModifiedBy equals submittedByUser.Id into submittedByUserJoin
                from submittedByUser in submittedByUserJoin.DefaultIfEmpty()

                where data.AdvisoryDate >= request.StartDate
                && data.AdvisoryDate <= request.EndDate && data.TenantId.Length > 6

                select new ActivitiesAdvisoriesTabularData
                {
                    ContractName =
                        (from con in context.Contracts
                        where data.TenantId.StartsWith(con.Tenant!.Id)
                        orderby con.Tenant!.Id.Length descending
                        select con.Description)
                        .FirstOrDefault(),

                    ParticipantId = data.ParticipantId,
                    Queue = data.Queue,
                    ActivityType = a.Type,
                    SupportWorker = sw.DisplayName,
                    CfoUser = cfoUser.DisplayName,

                    PqaSubmittedDate = latestSubmission != null
                        ? (DateTime?)latestSubmission.LastModified
                        : null,
                    PqaUser = submittedByUser != null
                        ? submittedByUser.DisplayName
                        : null,

                    AdvisoryDate = data.AdvisoryDate,
                    IsCompleted = data.IsCompleted,
                    IsAccepted = data.IsAccepted ? "Yes" : "No",
                    Escalated = data.Escalated == true ? "Yes"
                            : data.Escalated == false ? "No"
                            : "",
                    Message = (data.Message ?? "").Replace("\r", " ").Replace("\n", " ")
                };

            var result = await query.OrderBy(r => r.AdvisoryDate)
                            .AsNoTracking()
                            .ToArrayAsync(cancellationToken);

            return new ActivitiesAdvisoriesToProviderDto(result);

        }

    }

    public record ActivitiesAdvisoriesToProviderDto
    {
        public ActivitiesAdvisoriesToProviderDto(ActivitiesAdvisoriesTabularData[] tabularData)
        {
            TabularData = tabularData;
            
            ChartData = tabularData
                .GroupBy(td => new { td.ContractName, td.Queue, td.ActivityType })
                .OrderBy(g => g.Key.ContractName)
                .ThenBy(g => g.Key.Queue)
                .ThenBy(g => g.Key.ActivityType?.Name)
                .Select(g => new ActivitiesAdvisoriesChartData
                {
                    ContractName = g.Key.ContractName,
                    Queue = g.Key.Queue,
                    ActivityType = g.Key.ActivityType,
                    Count = g.Count()
                })
                .ToArray();
        }
        public ActivitiesAdvisoriesTabularData[] TabularData { get;}
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
        public string? Queue { get; set; }
        public int Count { get; set; }
    }

}