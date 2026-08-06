using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries.Extensions;
using Cfo.Cats.Application.Features.Participants.Specifications;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Labels;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class ParticipantsWithPagination
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : PaginationFilter, IQuery<Result<PaginatedData<ParticipantPaginationDto>>>
    {
        /// <summary>
        /// The filter for the list (based on the status)
        /// </summary>
        public ParticipantListView ListView { get; set; } = ParticipantListView.Default;

        /// <summary>
        /// The currently logged-in user
        /// </summary>
        public UserProfile? CurrentUser { get; set; }

        /// <summary>
        /// Flag to indicate that you only want to see your own cases.
        /// </summary>
        [Description("Just My Cases")]
        public bool JustMyCases { get; set; } = true;

        /// <summary>
        /// The current location of the participant
        /// </summary>
        public int[] Locations { get; set; } = [];
        public LabelId? Label { get; set; }
        public string? OwnerId { get; set; }
        public string? TenantId { get; set; }
        public DateTime? RiskDue { get; set; }
        public RecentParticipantFilter RecentAction { get; set; } = RecentParticipantFilter.All;
    }

    public class Handler(IUnitOfWork unitOfWork) : IQueryHandler<Query, Result<PaginatedData<ParticipantPaginationDto>>>
    {
        public async Task<Result<PaginatedData<ParticipantPaginationDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            // Start with base tenant filter and compose all filters
            var query = context.Participants
                .Where(p => p.Owner!.TenantId!.StartsWith(request.CurrentUser!.TenantId!))
                .ApplyKeywordSearch(request.Keyword)
                .ApplyLocationFilter(request.Locations)
                .ApplyListViewFilter(request.ListView)
                .ApplyLabelFilter(request.Label, context)
                .ApplyOwnershipFilter(request.JustMyCases, request.OwnerId, request.TenantId, request.CurrentUser!.UserId)
                .ApplyRiskDueFilter(request.RiskDue)
                .ApplyRecentActionFilter(request.RecentAction, request.CurrentUser!.UserId, context);

            var count = await query.AsNoTracking().CountAsync(cancellationToken);

            // Project to DTO and apply sorting
            var data = await query
                .ProjectToPaginationDto(context, request.RecentAction, request.CurrentUser!.UserId)
                .AsNoTracking()
                .ApplySmartSorting(request.OrderBy, request.SortDirection, request.RecentAction, request.RiskDue)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedData<ParticipantPaginationDto>(data, count, request.PageNumber, request.PageSize);
        }
    }
    
    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
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
}
