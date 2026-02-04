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

            
            var query = from pfa in context.ProviderFeedbackEnrolments
                where pfa.Message != null
                    && (pfa.FeedbackType == 0 || pfa.FeedbackType == 1)
                    && pfa.ActionDate >= request.StartDate
                    && pfa.ActionDate < request.EndDate.AddDays(1)
                    && (!string.IsNullOrWhiteSpace(request.TenantId) ? pfa.TenantId.StartsWith(request.TenantId) : true)
                join cfoUser in context.Users on pfa.CfoUserId equals cfoUser.Id into cfoUserJoin
                from cfoUser in cfoUserJoin.DefaultIfEmpty()
                join sw in context.Users on pfa.SupportWorkerId equals sw.Id into swJoin
                from sw in swJoin.DefaultIfEmpty()
                join submittedByUser in context.Users on pfa.ProviderQaUserId equals submittedByUser.Id into submittedByUserJoin
                from submittedByUser in submittedByUserJoin.DefaultIfEmpty()                

                select new EnrolmentAdvisoriesTabularData
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
                    AdvisoryDate = pfa.ActionDate,
                    Message = (pfa.Message ?? "").Replace("\r", " ").Replace("\n", " ")
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
        public string? Message { get; set; }
    }
    public record EnrolmentAdvisoriesChartData
    {
        public string? ContractName { get; set; }
        public int EscalationQueue { get; set; }
        public int QA2Queue { get; set; }
    }

}