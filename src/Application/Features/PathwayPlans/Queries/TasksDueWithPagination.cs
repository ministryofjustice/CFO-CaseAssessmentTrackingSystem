using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.PathwayPlans.Queries;

/// <summary>
/// Categorises an outstanding <see cref="Domain.Entities.Participants.ObjectiveTask"/> based on how close its
/// due date is, so that Support Workers (and PQAs, via the owner filter) can keep on top of their caseload
/// without needing to track this information separately.
/// </summary>
public enum TaskDueCategory
{
    /// <summary>
    /// The task's due date has passed and it has not been completed.
    /// </summary>
    Overdue,

    /// <summary>
    /// The task is due within the next 7 days.
    /// </summary>
    DueImminently,

    /// <summary>
    /// The task is due within the next 30 days (but not the next 7).
    /// </summary>
    DueSoon
}

public static class TasksDueWithPagination
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : PaginationFilter, IQuery<Result<PaginatedData<TaskDueDto>>>
    {
        /// <summary>
        /// The currently logged-in user.
        /// </summary>
        public required UserProfile CurrentUser { get; set; }

        /// <summary>
        /// Filter to a specific caseworker's caseload. Support Workers default to their own Id;
        /// PQAs (and other roles) can use this to review another Support Worker's caseload.
        /// </summary>
        public string? OwnerId { get; set; }

        /// <summary>
        /// Filter to a specific tenant (organisation/team).
        /// </summary>
        public string? TenantId { get; set; }

        /// <summary>
        /// Restrict results to a single due-date category. When null, all outstanding tasks due within
        /// the next 30 days (or overdue) are returned.
        /// </summary>
        public TaskDueCategory? Category { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IQueryHandler<Query, Result<PaginatedData<TaskDueDto>>>
    {
        public async Task<Result<PaginatedData<TaskDueDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var today = DateTime.UtcNow.Date;
            var dueSoonCutoff = today.AddDays(30);

            var query = from task in context.ObjectiveTasks
                        where task.Completed == null && task.Due < dueSoonCutoff.AddDays(1)
                        join objective in context.Objectives on task.ObjectiveId equals objective.Id
                        join pathwayPlan in context.PathwayPlans on objective.PathwayPlanId equals pathwayPlan.Id
                        join participant in context.Participants on pathwayPlan.ParticipantId equals participant.Id
                        where participant.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value
                        join owner in context.Users on participant.OwnerId equals owner.Id into owners
                        from owner in owners.DefaultIfEmpty()
                        select new TaskDueDto
                        {
                            TaskId = task.Id,
                            TaskDescription = task.Description,
                            Due = task.Due,
                            IsMandatory = task.IsMandatory,
                            ObjectiveId = objective.Id,
                            ObjectiveDescription = objective.Description,
                            ParticipantId = participant.Id,
                            ParticipantName = $"{participant.FirstName} {participant.LastName}",
                            OwnerId = participant.OwnerId,
                            OwnerName = owner != null ? owner.DisplayName : null,
                            TenantId = owner != null ? owner.TenantId : null
                        };

            // Always scope results to the current user's tenant tree (e.g. a Support Worker's own
            // tenant, or a broader tenant for roles with wider visibility). This prevents users from
            // seeing tasks belonging to participants owned by other, unrelated contracts/tenants.
            query = query.Where(x => x.TenantId != null && x.TenantId.StartsWith(request.CurrentUser.TenantId!));

            if (!string.IsNullOrWhiteSpace(request.OwnerId))
            {
                query = query.Where(x => x.OwnerId == request.OwnerId);
            }
            else if (!string.IsNullOrWhiteSpace(request.TenantId))
            {
                query = query.Where(x => x.TenantId != null && x.TenantId.StartsWith(request.TenantId));
            }

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                query = query.Where(x => x.ParticipantName.Contains(request.Keyword)
                    || x.ParticipantId.Contains(request.Keyword)
                    || x.TaskDescription.Contains(request.Keyword));
            }

            if (request.Category is { } category)
            {
                query = category switch
                {
                    TaskDueCategory.Overdue => query.Where(x => x.Due < today),
                    TaskDueCategory.DueImminently => query.Where(x => x.Due >= today && x.Due < today.AddDays(8)),
                    TaskDueCategory.DueSoon => query.Where(x => x.Due >= today.AddDays(8) && x.Due < dueSoonCutoff.AddDays(1)),
                    _ => query
                };
            }

            var count = await query.CountAsync(cancellationToken);

            var sortColumn = request.OrderBy.Trim().ToLowerInvariant() switch
            {
                "due" => "Due",
                "participantname" => "ParticipantName",
                "ownername" => "OwnerName",
                _ => "Due"
            };

            var sortDirection = request.SortDirection.Equals("Ascending", StringComparison.OrdinalIgnoreCase)
                ? "ascending"
                : "descending";

            var data = await query
                .AsNoTracking()
                .OrderBy($"{sortColumn} {sortDirection}")
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedData<TaskDueDto>(data, count, request.PageNumber, request.PageSize);
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(r => r.CurrentUser.TenantId)
                .NotNull()
                .WithMessage("CurrentUser must have a TenantId.");

            RuleFor(r => r.Keyword)
                .Matches(ValidationConstants.Keyword)
                .WithMessage(string.Format(ValidationConstants.KeywordMessage, "Search Keyword"));

            RuleFor(r => r.PageNumber)
                .GreaterThan(0)
                .WithMessage(string.Format(ValidationConstants.PositiveNumberMessage, "Page Number"));

            RuleFor(r => r.PageSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(ValidationConstants.MaximumPageSize)
                .WithMessage(ValidationConstants.MaximumPageSizeMessage);

            RuleFor(r => r.SortDirection)
                .Matches(ValidationConstants.SortDirection)
                .WithMessage(ValidationConstants.SortDirectionMessage);

            RuleFor(r => r.OrderBy)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "OrderBy"));
        }
    }

    public class TaskDueDto
    {
        public required Guid TaskId { get; init; }
        public required string TaskDescription { get; init; }
        public required DateTime Due { get; init; }
        public required bool IsMandatory { get; init; }
        public required Guid ObjectiveId { get; init; }
        public required string ObjectiveDescription { get; init; }
        public required string ParticipantId { get; init; }
        public required string ParticipantName { get; init; }
        public string? OwnerId { get; init; }
        public string? OwnerName { get; init; }
        public string? TenantId { get; init; }

        public TaskDueCategory Category => Due.Date < DateTime.UtcNow.Date
            ? TaskDueCategory.Overdue
            : Due.Date < DateTime.UtcNow.Date.AddDays(8)
                ? TaskDueCategory.DueImminently
                : TaskDueCategory.DueSoon;
    }
}
