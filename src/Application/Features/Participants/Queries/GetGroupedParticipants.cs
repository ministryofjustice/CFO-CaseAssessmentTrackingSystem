using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Specifications;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

/// <summary>
/// Returns a single page of participant groups (e.g. assignees) for the supplied filter. Only the
/// group key, label and total count are returned — the participant rows are fetched on demand when a
/// group is expanded. The groups themselves are paginated, so the pager reveals more assignees.
/// </summary>
public static class GetGroupedParticipants
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IQuery<Result<GroupedParticipantsDto>>
    {
        /// <summary>
        /// The underlying participant filter (carries the active <see cref="ParticipantGroupBy"/>).
        /// </summary>
        public required ParticipantsWithPagination.Query Filter { get; set; }

        /// <summary>
        /// The current page of groups (1-based).
        /// </summary>
        public int GroupPageNumber { get; set; } = 1;

        /// <summary>
        /// The number of groups (e.g. assignees) per page.
        /// </summary>
        public int GroupPageSize { get; set; } = 10;
    }

    public class Handler(IUnitOfWork unitOfWork) : IQueryHandler<Query, Result<GroupedParticipantsDto>>
    {
        public async Task<Result<GroupedParticipantsDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var filter = request.Filter;

            if (filter.GroupBy == ParticipantGroupBy.None)
            {
                return Result<GroupedParticipantsDto>.Success(new GroupedParticipantsDto());
            }

            var context = unitOfWork.DbContext;
            var baseQuery = ParticipantsWithPagination.Handler.ApplyFilter(context, filter);

            // Aggregate to one row per group (key + label + total count), paginated across the groups.
            var groups = filter.GroupBy switch
            {
                ParticipantGroupBy.Assignee => baseQuery
                    .GroupBy(p => new { p.OwnerId, p.Owner!.DisplayName })
                    .Select(g => new ParticipantGroupDto { Key = g.Key.OwnerId!, Label = g.Key.DisplayName!, Count = g.Count() }),
                ParticipantGroupBy.Tenant => baseQuery
                    .GroupBy(p => new { p.Owner!.TenantId, p.Owner!.TenantName })
                    .Select(g => new ParticipantGroupDto { Key = g.Key.TenantId!, Label = g.Key.TenantName!, Count = g.Count() }),
                _ => throw new ArgumentOutOfRangeException(nameof(filter.GroupBy))
            };

            var totalGroups = await groups.CountAsync(cancellationToken);

            var pageGroups = await groups
                .OrderBy(g => g.Label)
                .Skip((request.GroupPageNumber - 1) * request.GroupPageSize)
                .Take(request.GroupPageSize)
                .ToListAsync(cancellationToken);

            return Result<GroupedParticipantsDto>.Success(new GroupedParticipantsDto
            {
                TotalGroups = totalGroups,
                Groups = pageGroups.ToArray()
            });
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(q => q.Filter)
                .NotNull();

            RuleFor(q => q.GroupPageNumber)
                .GreaterThan(0);

            RuleFor(q => q.GroupPageSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(ValidationConstants.MaximumPageSize);
        }
    }
}
