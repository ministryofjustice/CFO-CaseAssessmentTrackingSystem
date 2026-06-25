using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Specifications;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

/// <summary>
/// Returns a single page of participant groups (e.g. assignees) for the supplied filter.
/// The groups themselves are paginated (so the pager reveals more assignees); each group
/// carries all of its participant rows.
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

    private class GroupHeader
    {
        public required string Key { get; set; }
        public required string Label { get; set; }
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

            // The distinct groups (key + label), used to paginate across the groups themselves.
            var groups = filter.GroupBy switch
            {
                ParticipantGroupBy.Assignee => baseQuery
                    .Select(p => new GroupHeader { Key = p.OwnerId!, Label = p.Owner!.DisplayName! }),
                ParticipantGroupBy.Tenant => baseQuery
                    .Select(p => new GroupHeader { Key = p.Owner!.TenantId!, Label = p.Owner!.TenantName! }),
                _ => throw new ArgumentOutOfRangeException(nameof(filter.GroupBy))
            };

            groups = groups.Distinct();

            var totalGroups = await groups.CountAsync(cancellationToken);

            var pageGroups = await groups
                .OrderBy(g => g.Label)
                .Skip((request.GroupPageNumber - 1) * request.GroupPageSize)
                .Take(request.GroupPageSize)
                .ToListAsync(cancellationToken);

            var rowOrder = ParticipantsWithPagination.Handler.BuildOrderExpression(filter, includeGroup: false);

            var result = new List<ParticipantGroupDto>(pageGroups.Count);

            foreach (var group in pageGroups)
            {
                var groupQuery = filter.GroupBy == ParticipantGroupBy.Assignee
                    ? baseQuery.Where(p => p.OwnerId == group.Key)
                    : baseQuery.Where(p => p.Owner!.TenantId == group.Key);

                var items = await ParticipantsWithPagination.Handler
                    .ProjectToDto(context, groupQuery, filter)
                    .OrderBy(rowOrder)
                    .ToListAsync(cancellationToken);

                result.Add(new ParticipantGroupDto
                {
                    Label = group.Label,
                    Items = items.ToArray()
                });
            }

            return Result<GroupedParticipantsDto>.Success(new GroupedParticipantsDto
            {
                TotalGroups = totalGroups,
                Groups = result.ToArray()
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
