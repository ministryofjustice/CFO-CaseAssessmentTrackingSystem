using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetEnrolmentsToProvider
{
    [RequestAuthorize(Policy = SecurityPolicies.Internal)]
    public class Query : IRequest<Result<EnrolmentToProviderDto>>
    {
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public string? UserId { get; set; }
        public string? TenantId { get; set; }
        public required UserProfile CurrentUser { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<EnrolmentToProviderDto>>
    {
        
        public async Task<Result<EnrolmentToProviderDto>> Handle(Query request, CancellationToken cancellationToken)
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
                    ReturnedDate = q.Created,
                    SupportWorkerId = q.SupportWorkerId,
                    TenantId = q.TenantId
                });

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
                    ReturnedDate = q.Created,
                    SupportWorkerId = q.SupportWorkerId,
                    TenantId = q.TenantId
                });

            var combined = qa2Data.Select(x => new
            {
                x.Queue,
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

                from latestSubmission in context.ParticipantEnrolmentHistories
                    .Where(eh =>
                        eh.ParticipantId == data.ParticipantId &&
                        eh.EnrolmentStatus == 2 &&
                        eh.Created < data.ReturnedDate)
                    .OrderByDescending(eh => eh.Created)
                    .Take(1)
                    .DefaultIfEmpty() 

                join submittedByUser in context.Users on latestSubmission.CreatedBy equals submittedByUser.Id into submittedByUserJoin
                from submittedByUser in submittedByUserJoin.DefaultIfEmpty()

                where data.ReturnedDate >= request.StartDate
                && data.ReturnedDate <= request.EndDate
                && data.IsCompleted == true
                && data.IsAccepted == false

                select new EnrolmentsTabularData
                {
                    ContractName = c.Description,
                    ParticipantId = data.ParticipantId,
                    Queue = data.Queue,
                    SupportWorker = sw.DisplayName,
                    CfoUser = cfoUser.DisplayName,

                    // pulled from the SAME EnrolmentHistory row
                    SubmittedDate = latestSubmission != null
                        ? (DateTime?)latestSubmission.Created
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

            return new EnrolmentToProviderDto(result);

        }

    }

    public record EnrolmentToProviderDto
    {
        public EnrolmentToProviderDto(EnrolmentsTabularData[] tabularData)
        {
            TabularData = tabularData;
            
            // Get all unique contracts first, sorted
            var allContracts = tabularData
                .Select(td => td.ContractName)
                .Distinct()
                .OrderBy(c => c)
                .ToList();
            
            // Create chart data ensuring all queues have entries for all contracts
            var grouped = tabularData
                .GroupBy(td => new { td.ContractName, td.Queue })
                .Select(g => new EnrolmentsChartData
                {
                    ContractName = g.Key.ContractName,
                    Queue = g.Key.Queue,
                    Count = g.Count()
                })
                .ToList();
            
            // Always ensure both queues exist (Escalation and QA2)
            var queues = new List<string> { "Escalation", "QA2" };
            var completeData = new List<EnrolmentsChartData>();
            
            foreach (var queue in queues)
            {
                foreach (var contract in allContracts)
                {
                    var existing = grouped.FirstOrDefault(x => x.Queue == queue && x.ContractName == contract);
                    completeData.Add(existing ?? new EnrolmentsChartData
                    {
                        ContractName = contract,
                        Queue = queue,
                        Count = 0
                    });
                }
            }
            
            ChartData = completeData.ToArray();
        }
        public EnrolmentsTabularData[] TabularData { get;}
        public EnrolmentsChartData[] ChartData { get;}
    }

    public record EnrolmentsTabularData
    {
        public string? TenantId { get; set; }
        public string? ContractName { get; set; }
        public string? ParticipantId { get; set; }
        public string? Queue { get; set; }
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
    public record EnrolmentsChartData
    {
        public string? ContractName { get; set; }
        public string? Queue { get; set; }
        public int Count { get; set; }
    }

}