using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetActivitiesFeedback
{
    [RequestAuthorize(Policy = SecurityPolicies.Internal)]
    public class Query : IRequest<Result<ActivitiesFeedbackDto>>
    {
        public string? TenantId { get; init; }
        public required UserProfile CurrentUser { get; init; }
        public required DateTime StartDate { get; init; }
        public required DateTime EndDate { get; init; }
        public string? UserId { get; init; }
        public bool ShowRead { get; init; }
    }

    public class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<ActivitiesFeedbackDto>>
    {
        public async Task<Result<ActivitiesFeedbackDto>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var query =
                from afb in context.ActivityFeedbacks.AsNoTracking()

                join p in context.Participants on afb.ParticipantId equals p.Id
                join ru in context.Users on afb.OwnerId equals ru.Id into ruJoin
                from ru in ruJoin.DefaultIfEmpty()

                join cu in context.Users on afb.CreatedBy equals cu.Id into cuJoin
                from cu in cuJoin.DefaultIfEmpty()

                where afb.Created >= request.StartDate.Date
                        && afb.Created < request.EndDate.AddDays(1).Date
                        && (request.UserId == null || afb.OwnerId == request.UserId)
        
                select new { afb, p, ru, cu };

            var tabularData = await query
                .OrderBy(x => x.afb.Qa1Date)
                .Select(x => new ActivitiesFeedbackTabularData
                {
                    Id = x.afb.Id,
                    ParticipantId = x.afb.ParticipantId,
                    FirstName = x.p.FirstName,
                    LastName = x.p.LastName,
                    
                    RecipientUser = x.ru != null ? x.ru.DisplayName! : "Unknown",
                    ActivityCategory = x.afb.ActivityCategory,
                    ActivityType = x.afb.ActivityType,
                    ActivityFeedbackReason = x.afb.ActivityFeedbackReason,

                    Queue = x.afb.Stage.Name ?? "Unknown",
                    Qa1Outcome = x.afb.Qa1Outcome.Name,
                    Outcome = x.afb.Outcome.Name,

                    QaUser = x.cu != null
                        ? x.cu.DisplayName!
                        : "Unknown",

                    Created = x.afb.Created, 

                    ActivityProcessedDate = x.afb.Qa1Date,
 
                    IsRead = x.afb.IsRead,
                    Message = x.afb.Message
                })
                .ToArrayAsync(cancellationToken);

            return Result<ActivitiesFeedbackDto>
                .Success(new ActivitiesFeedbackDto(tabularData));
        }
    }

    public record ActivitiesFeedbackDto
    {
        public ActivitiesFeedbackDto(
            ActivitiesFeedbackTabularData[] tabularData)
        {
            TabularData = tabularData;

            ChartData = tabularData
                .GroupBy(td => new { td.RecipientUser, td.ActivityFeedbackReason })
                .OrderBy(g => g.Key.RecipientUser)
                .ThenBy(g => g.Key.ActivityFeedbackReason)
                .Select(g => new ActivitiesFeedbackChartData
                {
                    Recipient = g.Key.RecipientUser,
                    ActivityFeedbackReason = g.Key.ActivityFeedbackReason,
                    Count = g.Count()
                })
                .ToArray();
        }

        public ActivitiesFeedbackTabularData[] TabularData { get; }
        public ActivitiesFeedbackChartData[] ChartData { get; }
    }

    public record ActivitiesFeedbackTabularData
    {
        public Guid Id { get; init; }   // 👈 ADD THIS
        public bool ShowDetails { get; set; } 
        public string? ParticipantId { get; init; }
        public required string RecipientUser { get; init; }
        public string? FirstName { get; init; }
        public string? LastName { get; init; }

        public required string ActivityCategory { get; init; }
        public required string ActivityType { get; init; }
        public required string ActivityFeedbackReason { get; init; }

        public required string Queue { get; init; }
        
        public required string Qa1Outcome { get; init; }
        public required string Outcome { get; init; }
        public required string QaUser { get; init; }
       
        // public required DateTime? LastModified { get; set; }
        public DateTime? Created { get; init; } 
        public DateTime ActivityProcessedDate { get; init; }

        public required string Message { get; init; }
        public required bool IsRead { get; set; }
    }

    public record ActivitiesFeedbackChartData
    {
        public string? Recipient { get; init; }
        public string? ActivityFeedbackReason { get; init; }
        public int Count { get; init; }
    }
}