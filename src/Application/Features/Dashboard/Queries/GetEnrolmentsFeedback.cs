using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetEnrolmentsFeedback
{
    [RequestAuthorize(Policy = SecurityPolicies.Internal)]
    public class Query : IRequest<Result<EnrolmentsFeedbackDto>>
    {
        public string? TenantId { get; init; }
        public required UserProfile CurrentUser { get; init; }
        public required DateTime StartDate { get; init; }
        public required DateTime EndDate { get; init; }
        public string? UserId { get; init; }
        public bool ShowRead { get; init; }
    }

    public class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<EnrolmentsFeedbackDto>>
    {
        public async Task<Result<EnrolmentsFeedbackDto>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var query =
                from efb in context.EnrolmentFeedbacks.AsNoTracking()

                join p in context.Participants on efb.ParticipantId equals p.Id
                join ru in context.Users on efb.OwnerId equals ru.Id into ruJoin
                from ru in ruJoin.DefaultIfEmpty()

                join cu in context.Users on efb.CreatedBy equals cu.Id into cuJoin
                from cu in cuJoin.DefaultIfEmpty()

                where efb.Created >= request.StartDate.Date
                        && efb.Created < request.EndDate.AddDays(1).Date
                        && (request.UserId == null || efb.OwnerId == request.UserId)
        
                select new { efb, p, ru, cu };

            var tabularData = await query
                .OrderBy(x => x.efb.Qa1Date)
                .Select(x => new EnrolmentsFeedbackTabularData
                {
                    Id = x.efb.Id,
                    ParticipantId = x.efb.ParticipantId,
                    FirstName = x.p.FirstName,
                    LastName = x.p.LastName,
                    
                    RecipientUser = x.ru != null ? x.ru.DisplayName! : "Unknown",
                    EnrolmentFeedbackReason = x.efb.EnrolmentFeedbackReason,

                    Queue = x.efb.Stage.Name ?? "Unknown",
                    Qa1Outcome = x.efb.Qa1Outcome.Name,
                    Outcome = x.efb.Outcome.Name,

                    QaUser = x.cu != null
                        ? x.cu.DisplayName!
                        : "Unknown",

                    Created = x.efb.Created, 

                    EnrolmentProcessedDate = x.efb.Qa1Date,
 
                    IsRead = x.efb.IsRead,
                    Message = x.efb.Message
                })
                .ToArrayAsync(cancellationToken);

            return Result<EnrolmentsFeedbackDto>
                .Success(new EnrolmentsFeedbackDto(tabularData));
        }
    }

    public record EnrolmentsFeedbackDto
    {
        public EnrolmentsFeedbackDto(
            EnrolmentsFeedbackTabularData[] tabularData)
        {
            TabularData = tabularData;

            ChartData = tabularData
                .GroupBy(td => new { td.RecipientUser, td.EnrolmentFeedbackReason })
                .OrderBy(g => g.Key.RecipientUser)
                .ThenBy(g => g.Key.EnrolmentFeedbackReason)
                .Select(g => new EnrolmentsFeedbackChartData
                {
                    Recipient = g.Key.RecipientUser,
                    EnrolmentFeedbackReason = g.Key.EnrolmentFeedbackReason,
                    Count = g.Count()
                })
                .ToArray();
        }

        public EnrolmentsFeedbackTabularData[] TabularData { get; }
        public EnrolmentsFeedbackChartData[] ChartData { get; }
    }

    public record EnrolmentsFeedbackTabularData
    {
        public Guid Id { get; init; }
        public bool ShowDetails { get; set; } 
        public string? ParticipantId { get; init; }
        public required string RecipientUser { get; init; }
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        
        public required string EnrolmentFeedbackReason { get; init; }

        public required string Queue { get; init; }
        
        public required string Qa1Outcome { get; init; }
        public required string Outcome { get; init; }
        public required string QaUser { get; init; }
       
        public DateTime? Created { get; init; } 
        public DateTime EnrolmentProcessedDate { get; init; }

        public required string Message { get; init; }
        public required bool IsRead { get; set; }
    }

    public record EnrolmentsFeedbackChartData
    {
        public string? Recipient { get; init; }
        public string? EnrolmentFeedbackReason { get; init; }
        public int Count { get; init; }
    }
}