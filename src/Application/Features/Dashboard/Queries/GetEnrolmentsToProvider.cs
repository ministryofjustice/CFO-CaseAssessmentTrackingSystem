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

            var query = from pfa in context.ProviderFeedbackEnrolments
                where pfa.FeedbackType == ((int)FeedbackType.Returned)
                    && pfa.ActionDate >= request.StartDate
                    && pfa.ActionDate < request.EndDate.AddDays(1)
                    && (!string.IsNullOrWhiteSpace(request.TenantId) ? pfa.TenantId.StartsWith(request.TenantId) : true)
                join cfoUser in context.Users on pfa.CfoUserId equals cfoUser.Id into cfoUserJoin
                from cfoUser in cfoUserJoin.DefaultIfEmpty()
                join sw in context.Users on pfa.SupportWorkerId equals sw.Id into swJoin
                from sw in swJoin.DefaultIfEmpty()
                join submittedByUser in context.Users on pfa.ProviderQaUserId equals submittedByUser.Id into submittedByUserJoin
                from submittedByUser in submittedByUserJoin.DefaultIfEmpty()                

                select new EnrolmentsTabularData
                {
                    ContractName =
                        (from con in context.Contracts
                        where pfa.TenantId.StartsWith(con.Tenant!.Id)
                        orderby con.Tenant!.Id.Length descending
                        select con.Description)
                        .FirstOrDefault(),
                    ParticipantId = pfa.ParticipantId,
                    Queue = pfa.Queue,
                    SupportWorker = sw.DisplayName,
                    CfoUser = cfoUser.DisplayName,
                    PqaSubmittedDate = pfa.PqaSubmittedDate,
                    PqaUser = submittedByUser.DisplayName,
                    ReturnedDate = pfa.ActionDate,
                    Message = (pfa.Message ?? "").Replace("\r", " ").Replace("\n", " ")
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

            var allContracts = tabularData
                .Select(td => td.ContractName)
                .Distinct()
                .OrderBy(c => c)
                .ToList();
            
            var grouped = tabularData
                .GroupBy(td => new { td.ContractName, td.Queue })
                .Select(g => new EnrolmentsChartData
                {
                    ContractName = g.Key.ContractName,
                    Queue = g.Key.Queue,
                    Count = g.Count()
                })
                .ToList();
            
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
        public DateTime? PqaSubmittedDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public string? Message { get; set; }
    }
    public record EnrolmentsChartData
    {
        public string? ContractName { get; set; }
        public string? Queue { get; set; }
        public int Count { get; set; }
    }

}