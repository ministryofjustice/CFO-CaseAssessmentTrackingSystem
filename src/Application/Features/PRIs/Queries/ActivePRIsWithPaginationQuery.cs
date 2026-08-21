using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.PRIs.DTOs;
using Cfo.Cats.Application.Features.PRIs.Queries.Extensions;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.PRIs.Queries;

public static class ActivePRIsWithPagination
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : PaginationFilter, IQuery<Result<PaginatedData<PRIPaginationDto>>>
    {
        /// <summary>
        /// The currently logged-in user
        /// </summary>
        public UserProfile? CurrentUser { get; set; }

        /// <summary>
        /// Filter to only show PRIs created by or assigned to the current user
        /// </summary>
        [Description("Just My PRIs")]
        public bool JustMyPris { get; set; } = false;

        /// <summary>    
        /// Flag to indicate that you only want to see your incoming PRI's.
        /// </summary>
        [Description("Incoming PRIs")]
        public bool IncludeIncoming { get; set; } = false;

        /// <summary>    
        /// Flag to indicate that you only want to see your outgoing PRI's.
        /// </summary>
        [Description("Outgoing PRIs")]
        public bool IncludeOutgoing { get; set; } = false;

        /// <summary>
        /// Filter by custody support worker (CreatedBy)
        /// </summary>
        [Description("Custody Support Worker")]
        public string? CustodySupportWorker { get; set; }

        /// <summary>
        /// Filter by community support worker (AssignedTo)
        /// </summary>
        [Description("Community Support Worker")]
        public string? CommunitySupportWorker { get; set; }

        /// <summary>
        /// Filter by expected release region
        /// </summary>
        [Description("Expected Release Region")]
        public int? ExpectedReleaseRegionId { get; set; }

        /// <summary>
        /// Filter by participant active status
        /// </summary>
        [Description("Active Status")]
        public bool? ActiveStatus { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IQueryHandler<Query, Result<PaginatedData<PRIPaginationDto>>>
    {
        public async Task<Result<PaginatedData<PRIPaginationDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;
            
            // Start with base query - active PRIs only
            var query = context.PRIs
                .Where(x => PriStatus.ActiveList.Contains(x.Status))
                .ApplyTenantFilter(request.CurrentUser!)
                .ApplyJustMyPrisFilter(request.JustMyPris, request.CurrentUser!.UserId)
                .ApplyUserFilter(request.IncludeOutgoing, request.IncludeIncoming, request.CurrentUser!.UserId, 
                    request.CustodySupportWorker, request.CommunitySupportWorker)
                .ApplyKeywordSearch(request.Keyword)
                .ApplyCustodyWorkerFilter(request.CustodySupportWorker)
                .ApplyCommunityWorkerFilter(request.CommunitySupportWorker)
                .ApplyRegionFilter(request.ExpectedReleaseRegionId)
                .ApplyActiveStatusFilter(request.ActiveStatus);

            var count = await query.AsNoTracking().CountAsync(cancellationToken);

            // Project to DTO and apply sorting
            var data = await query
                .ProjectTo<PRIPaginationDto>(mapper.ConfigurationProvider)
                .AsNoTracking()
                .OrderBy($"{request.OrderBy} {request.SortDirection}")
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedData<PRIPaginationDto>(data, count, request.PageNumber, request.PageSize);
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
