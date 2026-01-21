using System.ComponentModel.DataAnnotations;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetEnrolmentAdvisoriesToProvider
{
    [RequestAuthorize(Policy = SecurityPolicies.Internal)]
    public class Query : IRequest<Result<EnrolmentAdvisoriesToProviderDto>>
    {
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public string? UserId { get; set; }
        public string? TenantId { get; set; }
        public required UserProfile CurrentUser { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<EnrolmentAdvisoriesToProviderDto>>
    {
        
        public async Task<Result<EnrolmentAdvisoriesToProviderDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            
            var qa2Data = context.EnrolmentQa2Queue
                .AsNoTracking()
                .SelectMany(q => q.Notes, (q, n) => new 
                {
                    Queue = "QA2",
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

            var escData = context.EnrolmentEscalationQueue
                .AsNoTracking()
                .SelectMany(q => q.Notes, (q, n) => new 
                {
                    Queue = "Escalation",
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

                from latestSubmission in context.ParticipantEnrolmentHistories
                    .Where(eh =>
                        eh.ParticipantId == data.ParticipantId 
                        && eh.EnrolmentStatus == 2
                        && eh.Created < data.AdvisoryDate)
                    .OrderByDescending(eh => eh.Created)
                    .Take(1)
                    .DefaultIfEmpty() 

                join submittedByUser in context.Users on latestSubmission.CreatedBy equals submittedByUser.Id into submittedByUserJoin
                from submittedByUser in submittedByUserJoin.DefaultIfEmpty()

                where data.AdvisoryDate >= request.StartDate
                && data.AdvisoryDate <= request.EndDate && data.TenantId.Length > 6

                select new EnrolmentAdvisoriesTabularData
                {
                    ContractName =
                        (from con in context.Contracts
                        where data.TenantId.StartsWith(con.Tenant!.Id)
                        orderby con.Tenant!.Id.Length descending
                        select con.Description)
                        .FirstOrDefault(),

                    ParticipantId = data.ParticipantId,
                    Queue = data.Queue,
                    SupportWorker = sw.DisplayName,
                    CfoUser = cfoUser.DisplayName,

                    PqaSubmittedDate = latestSubmission != null
                        ? (DateTime?)latestSubmission.Created
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

            return new EnrolmentAdvisoriesToProviderDto(result);

        }

    }

    public record EnrolmentAdvisoriesToProviderDto
    {
        public EnrolmentAdvisoriesToProviderDto(EnrolmentAdvisoriesTabularData[] tabularData)
        {
            TabularData = tabularData;
            
            ChartData = tabularData
                .GroupBy(td => td.ContractName)
                .OrderBy(g => g.Key)
                .Select(g => new EnrolmentAdvisoriesChartData
                {
                    ContractName = g.Key,
                    EscalationQueue = g.Count(x => x.Queue == "Escalation"),
                    QA2Queue = g.Count(x => x.Queue == "QA2")
                })
                .ToArray();
        }
        public EnrolmentAdvisoriesTabularData[] TabularData { get;}
        public EnrolmentAdvisoriesChartData[] ChartData { get;}
    }

    public record EnrolmentAdvisoriesTabularData
    {
        public string? TenantId { get; set; }
        public string? ContractName { get; set; }
        public string? ParticipantId { get; set; }
        public string? Queue { get; set; }
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
    public record EnrolmentAdvisoriesChartData
    {
        public string? ContractName { get; set; }
        public int EscalationQueue { get; set; }
        public int QA2Queue { get; set; }
    }

}