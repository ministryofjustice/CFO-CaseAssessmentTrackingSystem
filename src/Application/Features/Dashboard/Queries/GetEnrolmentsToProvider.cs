using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetEnrolmentsToProvider
{
    [RequestAuthorize(Policy = SecurityPolicies.Internal)]
    public class Query : IRequest<Result<EnrolmentToProviderDto[]>>
    {
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public string? UserId { get; set; }
        public required UserProfile CurrentUser { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<EnrolmentToProviderDto[]>>
    {
        
        public async Task<Result<EnrolmentToProviderDto[]>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            
        var qa2Data = context.EnrolmentQa2Queue
            .AsNoTracking()
            .SelectMany(q => q.Notes, (q, n) => new 
            {
                Queue = "QA2",
                ParticipantId = q.ParticipantId,
                PqaUser = q.OwnerId,
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
                PqaUser = q.CreatedBy,
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
            x.PqaUser,
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
            x.PqaUser,
            x.IsAccepted,
            x.IsCompleted,
            x.Message,
            x.Escalated,
            x.ReturnedDate,
            x.SupportWorkerId,
            x.TenantId
        }));

            var query = from data in combined
                        join qauser in context.Users on data.PqaUser equals qauser.Id into qauserJoin
                        from qauser in qauserJoin.DefaultIfEmpty()
                        join sw in context.Users on data.SupportWorkerId equals sw.Id into swJoin
                        from sw in swJoin.DefaultIfEmpty()
                        join c in context.Contracts on data.TenantId equals c.Tenant!.Id
                        where data.ReturnedDate >= request.StartDate && data.ReturnedDate <= request.EndDate
                            && data.IsCompleted == true && data.IsAccepted == false
                        select new EnrolmentToProviderDto
                        {
                            ContractName = c.Description,
                            ParticipantId = data.ParticipantId,
                            Queue = data.Queue,
                            SupportWorker = sw.DisplayName,
                            PqaUser = qauser.DisplayName,
                            SubmittedDate = context.ParticipantEnrolmentHistories
                                .Where(eh => eh.ParticipantId == data.ParticipantId
                                            && eh.EnrolmentStatus == 2
                                            && eh.Created < data.ReturnedDate)
                                .OrderByDescending(eh => eh.Created)
                                .Select(eh => (DateTime?)eh.Created)
                                .FirstOrDefault(),
                            ReturnedDate = data.ReturnedDate,
                            IsCompleted = data.IsCompleted,
                            IsAccepted = data.IsAccepted ? "Yes" : "No",
                            Escalated = data.Escalated == true ? "Yes" :
                                        data.Escalated == false ? "No" : "",
                            Message = (data.Message ?? "").Replace("\r", " ").Replace("\n", " ")
                        };

            var result = await query.OrderBy(r => r.ReturnedDate)
                            .AsNoTracking()
                            .ToArrayAsync(cancellationToken);

            return result.Length == 0
                ? Result<EnrolmentToProviderDto[]>.Failure("No enrolments to provider found for the specified criteria.")
                : Result<EnrolmentToProviderDto[]>.Success(result);

        }

    }

    public record EnrolmentToProviderDto
    {

        public string? TenantId { get; set; }
        public string? ContractName { get; set; }
        public string? ParticipantId { get; set; }
        public string? Queue { get; set; }
        public string? SupportWorkerId { get; set; }
        public string? SupportWorker { get; set; }
        public string? PqaUser { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public bool IsCompleted { get; set; }
        public string? IsAccepted { get; set; }
        public string? Escalated { get; set; }
        public string? Message { get; set; }

    }

}