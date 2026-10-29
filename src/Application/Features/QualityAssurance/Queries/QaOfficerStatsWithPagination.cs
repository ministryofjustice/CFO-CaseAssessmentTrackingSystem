using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.QualityAssurance.DTOs;
using Cfo.Cats.Application.Features.QualityAssurance.Specifications;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.QualityAssurance.Queries;

public static class QaOfficerStatsWithPagination
{
    [RequestAuthorize(Policy = SecurityPolicies.ServiceDeskManagement)]
    public class Query : PaginationFilter, IQuery<Result<PaginatedData<QaOfficerStatDto>>>
    {
        public Query()
        {
            OrderBy = nameof(QaOfficerStatDto.LastModified);
            SortDirection = "Descending";
        }

        public required QaOfficerStatsFilter Filter { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IQueryHandler<Query, Result<PaginatedData<QaOfficerStatDto>>>
    {
        public async Task<Result<PaginatedData<QaOfficerStatDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = BuildQuery(unitOfWork.DbContext, request.Filter);

            var count = await query.CountAsync(cancellationToken);

            var descending = request.SortDirection.Equals("Descending", StringComparison.OrdinalIgnoreCase);

            var ordered = request.OrderBy.Trim().ToLowerInvariant() switch
            {
                "crn" => descending ? query.OrderByDescending(x => x.Crn) : query.OrderBy(x => x.Crn),
                "queue" => descending ? query.OrderByDescending(x => x.Queue) : query.OrderBy(x => x.Queue),
                "qauser" => descending ? query.OrderByDescending(x => x.QaUser) : query.OrderBy(x => x.QaUser),
                "isaccepted" => descending ? query.OrderByDescending(x => x.IsAccepted) : query.OrderBy(x => x.IsAccepted),
                _ => descending ? query.OrderByDescending(x => x.LastModified) : query.OrderBy(x => x.LastModified)
            };

            var data = await ordered
                .ThenBy(x => x.Crn)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedData<QaOfficerStatDto>(data, count, request.PageNumber, request.PageSize);
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(r => r.Filter.StartDate)
                .LessThanOrEqualTo(r => r.Filter.EndDate)
                .WithMessage("Start date must be on or before the end date.");

            RuleFor(r => r.Filter.IsWithinMaxRange())
                .Equal(true)
                .WithMessage(QaOfficerStatsFilter.MaxRangeMessage);

            RuleFor(r => r.Filter.Crn)
                .Matches(ValidationConstants.Keyword)
                .WithMessage(string.Format(ValidationConstants.KeywordMessage, "CRN"));

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

    public static IQueryable<QaOfficerStatDto> BuildQuery(IApplicationDbContext context, QaOfficerStatsFilter filter)
    {
        var queues = filter.Queues is { Count: > 0 }
            ? filter.Queues.Distinct().ToArray()
            : Enum.GetValues<QaStatsQueue>();

        var branches = queues
            .Select(queue => ApplyFilters(filter.Process == QaStatsProcess.Enrolments
                ? EnrolmentBranch(context, queue)
                : ActivityBranch(context, queue), filter))
            .ToList();

        return branches.Aggregate((current, next) => current.Concat(next));
    }

    private static IQueryable<QaOfficerStatDto> ApplyFilters(IQueryable<QaOfficerStatDto> query, QaOfficerStatsFilter filter)
    {
        var start = filter.StartDate.Date;
        var endExclusive = filter.EndDate.Date.AddDays(1);

        query = query.Where(x => x.LastModified >= start && x.LastModified < endExclusive);

        if (string.IsNullOrWhiteSpace(filter.Crn) is false)
        {
            var crn = filter.Crn.Trim();
            query = query.Where(x => x.Crn != null && x.Crn.Contains(crn));
        }

        if (string.IsNullOrWhiteSpace(filter.QaUserId) is false)
        {
            query = query.Where(x => x.QaUserId == filter.QaUserId);
        }

        if (filter.IsAccepted is { } isAccepted)
        {
            query = query.Where(x => x.IsAccepted == isAccepted);
        }

        if (filter.NotesOnly)
        {
            query = query.Where(x => x.Message != null && x.Message != "");
        }

        return query;
    }

    private static IQueryable<QaOfficerStatDto> EnrolmentBranch(IApplicationDbContext context, QaStatsQueue queue) => queue switch
    {
        QaStatsQueue.Qa1 =>
            from q in context.EnrolmentQa1Queue
            where q.IsCompleted
            join u in context.Users on q.OwnerId equals u.Id into owners
            from owner in owners.DefaultIfEmpty()
            from n in q.Notes.DefaultIfEmpty()
            select new QaOfficerStatDto
            {
                EntryId = q.Id,
                Crn = q.ParticipantId,
                Queue = "QA1",
                LastModified = q.LastModified,
                QaUserId = q.OwnerId,
                QaUser = owner != null ? owner.DisplayName : null,
                IsAccepted = q.IsAccepted,
                Escalated = null,
                Message = n != null ? n.Message : null
            },
        QaStatsQueue.Qa2 =>
            from q in context.EnrolmentQa2Queue
            where q.IsCompleted
            join u in context.Users on q.OwnerId equals u.Id into owners
            from owner in owners.DefaultIfEmpty()
            from n in q.Notes.DefaultIfEmpty()
            select new QaOfficerStatDto
            {
                EntryId = q.Id,
                Crn = q.ParticipantId,
                Queue = "QA2",
                LastModified = q.LastModified,
                QaUserId = q.OwnerId,
                QaUser = owner != null ? owner.DisplayName : null,
                IsAccepted = q.IsAccepted,
                Escalated = q.IsEscalated,
                Message = n != null ? n.Message : null
            },
        QaStatsQueue.Escalation =>
            from q in context.EnrolmentEscalationQueue
            where q.IsCompleted
            join u in context.Users on q.CreatedBy equals u.Id into creators
            from creator in creators.DefaultIfEmpty()
            from n in q.Notes.DefaultIfEmpty()
            select new QaOfficerStatDto
            {
                EntryId = q.Id,
                Crn = q.ParticipantId,
                Queue = "Esc",
                LastModified = q.LastModified,
                QaUserId = q.CreatedBy,
                QaUser = creator != null ? creator.DisplayName : null,
                IsAccepted = q.IsAccepted,
                Escalated = null,
                Message = n != null ? n.Message : null
            },
        _ => throw new ArgumentOutOfRangeException($"Cannot build query for queue {queue}")
    };

    private static IQueryable<QaOfficerStatDto> ActivityBranch(IApplicationDbContext context, QaStatsQueue queue) => queue switch
    {
        QaStatsQueue.Qa1 =>
            from q in context.ActivityQa1Queue
            where q.IsCompleted
            join u in context.Users on q.OwnerId equals u.Id into owners
            from owner in owners.DefaultIfEmpty()
            from n in q.Notes.DefaultIfEmpty()
            select new QaOfficerStatDto
            {
                EntryId = q.Id,
                Crn = q.ParticipantId,
                Queue = "QA1",
                LastModified = q.LastModified,
                QaUserId = q.OwnerId,
                QaUser = owner != null ? owner.DisplayName : null,
                IsAccepted = q.IsAccepted,
                Escalated = null,
                Message = n != null ? n.Message : null
            },
        QaStatsQueue.Qa2 =>
            from q in context.ActivityQa2Queue
            where q.IsCompleted
            join u in context.Users on q.OwnerId equals u.Id into owners
            from owner in owners.DefaultIfEmpty()
            from n in q.Notes.DefaultIfEmpty()
            select new QaOfficerStatDto
            {
                EntryId = q.Id,
                Crn = q.ParticipantId,
                Queue = "QA2",
                LastModified = q.LastModified,
                QaUserId = q.OwnerId,
                QaUser = owner != null ? owner.DisplayName : null,
                IsAccepted = q.IsAccepted,
                Escalated = q.IsEscalated,
                Message = n != null ? n.Message : null
            },
        QaStatsQueue.Escalation =>
            from q in context.ActivityEscalationQueue
            where q.IsCompleted
            join u in context.Users on q.CreatedBy equals u.Id into creators
            from creator in creators.DefaultIfEmpty()
            from n in q.Notes.DefaultIfEmpty()
            select new QaOfficerStatDto
            {
                EntryId = q.Id,
                Crn = q.ParticipantId,
                Queue = "Esc",
                LastModified = q.LastModified,
                QaUserId = q.CreatedBy,
                QaUser = creator != null ? creator.DisplayName : null,
                IsAccepted = q.IsAccepted,
                Escalated = null,
                Message = n != null ? n.Message : null
            },
        _ => throw new ArgumentOutOfRangeException($"Cannot build query for queue {queue}")
    };
}
