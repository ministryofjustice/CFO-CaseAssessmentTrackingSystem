using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Labels.DTOs;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Specifications;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Labels;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class ParticipantsWithPagination
{
    [RequestAuthorize(Policy = SecurityPolicies.CandidateSearch)]
    public class Query : PaginationFilter, IRequest<Result<PaginatedData<ParticipantPaginationDto>>>
    {
        /// <summary>
        /// The filter for the list (based on the status)
        /// </summary>
        public ParticipantListView ListView { get; set; } = ParticipantListView.Default;

        /// <summary>
        /// The currently logged in user
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
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<PaginatedData<ParticipantPaginationDto>>>
    {
        public async Task<Result<PaginatedData<ParticipantPaginationDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var query = from p in context.Participants
                        where p.Owner!.TenantId!.StartsWith(request.CurrentUser!.TenantId!)
                        select p;

            if (request.JustMyCases)
            {
                query = query.Where(q => q.OwnerId == request.CurrentUser!.UserId);
            }

            if (request.Locations.Length > 0)
            {
                query = query.Where(p =>
                    request.Locations.Contains(p.CurrentLocation.Id) || (p.EnrolmentLocation != null &&
                                                                         request.Locations.Contains(p.EnrolmentLocation
                                                                             .Id)));
            }

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                if(request.Keyword.Split(" ") is { Length: 2 } segments)
                {
                    query = query.Where( p=> p.FirstName.Contains(segments[0]) && p.LastName.Contains(segments[1]));
                }
                else
                {
                    query = query.Where(p => p.FirstName.Contains(request.Keyword)
                                || p.LastName!.Contains(request.Keyword)
                                || p.Id.Contains(request.Keyword)
                                || p.ExternalIdentifiers.Any(ei => ei.Value.Contains(request.Keyword)));
                    
                }
            }

            query = request.ListView switch
            {
                ParticipantListView.Default => query.Where(p =>
                    p.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value
                    && p.EnrolmentStatus != EnrolmentStatus.DormantStatus.Value),
                ParticipantListView.SubmittedToAny => query.Where(p =>
                    p.EnrolmentStatus == EnrolmentStatus.SubmittedToProviderStatus.Value ||
                    p.EnrolmentStatus == EnrolmentStatus.SubmittedToAuthorityStatus.Value),
                ParticipantListView.Identified => query.Where(p =>
                    p.EnrolmentStatus == EnrolmentStatus.IdentifiedStatus.Value),
                ParticipantListView.Enrolling => query.Where(p =>
                    p.EnrolmentStatus == EnrolmentStatus.EnrollingStatus.Value),
                ParticipantListView.SubmittedToProvider => query.Where(p =>
                    p.EnrolmentStatus == EnrolmentStatus.SubmittedToProviderStatus.Value),
                ParticipantListView.SubmittedToQa => query.Where(p =>
                    p.EnrolmentStatus == EnrolmentStatus.SubmittedToAuthorityStatus.Value),
                ParticipantListView.Approved => query.Where(p =>
                    p.EnrolmentStatus == EnrolmentStatus.ApprovedStatus.Value),
                ParticipantListView.Dormant =>
                    query.Where(p => p.EnrolmentStatus == EnrolmentStatus.DormantStatus.Value),
                ParticipantListView.Archived => query.Where(p =>
                    p.EnrolmentStatus == EnrolmentStatus.ArchivedStatus.Value),
                ParticipantListView.All => query,
                _ => throw new ArgumentOutOfRangeException()
            };

            if(request.Label is not null)
            {
                query = from p in query
                        join pl in context.ParticipantLabels on p.Id equals EF.Property<string>(pl, "_participantId")
                        where EF.Property<LabelId>(pl, "LabelId") == request.Label
                            && pl.Lifetime.EndDate > DateTime.UtcNow
                        select p;

                query = query.Distinct();
            }

            if(string.IsNullOrEmpty(request.OwnerId) == false)
            {
                query = query.Where(p => p.OwnerId == request.OwnerId);
            }

            if(string.IsNullOrEmpty(request.TenantId) == false)
            {
                query = query.Where(p => p.Owner!.TenantId!.StartsWith(request.TenantId));
            }

            if (request.RiskDue.HasValue)
            {
                query = query.Where(p => p.RiskDue <= request.RiskDue);
            }
    
            var count = await query.AsNoTracking().CountAsync(cancellationToken);

            var transformedQuery =
                from p in query
                select new ParticipantPaginationDto()
                {
                    EnrolmentStatus = p.EnrolmentStatus!,
                    Owner = p.Owner!.DisplayName!,
                    ConsentStatus = p.ConsentStatus!,
                    CurrentLocation = new LocationDto
                    {
                        Id = p.CurrentLocation.Id,
                        Name = p.CurrentLocation.Name,
                        GenderProvision = p.CurrentLocation.GenderProvision,
                        LocationType = p.CurrentLocation.LocationType,
                        ContractName = p.CurrentLocation.Contract!.Description
                    },
                    Id = p.Id,
                    EnrolmentLocation = p.EnrolmentLocation == null
                        ? null
                        : new LocationDto
                        {
                            Name = p.EnrolmentLocation.Name,
                            GenderProvision = p.EnrolmentLocation.GenderProvision,
                            LocationType = p.EnrolmentLocation.LocationType,
                            Id = p.EnrolmentLocation.Id,
                            ContractName = p.EnrolmentLocation.Contract!.Description
                        },
                    ParticipantName = p.FirstName + " " + p.LastName,
                    RiskDue = p.RiskDue,
                    RiskDueReason = p.RiskDueReason!,
                    Tenant = p.Owner!.TenantName!,
                    Labels = (
                            from pl in context.ParticipantLabels
                            where EF.Property<string>(pl, "_participantId") == p.Id
                                && pl.Lifetime.EndDate > DateTime.UtcNow
                            orderby pl.Lifetime.StartDate descending
                            select new LabelDto
                            {
                                Name = pl.Label.Name,
                                Description = pl.Label.Description,
                                Scope = pl.Label.Scope,
                                Contract = pl.Label.ContractId!,
                                Id = pl.Label.Id.Value,
                                AppIcon = pl.Label.AppIcon,
                                Colour = pl.Label.Colour,
                                Variant = pl.Label.Variant
                            }).ToArray(),
                    ConsentGranted = p.DateOfFirstConsent
                };

            var sortColumn = request.OrderBy.Trim().ToLowerInvariant() switch
            {
                "id" => "Id",
                "participantname" => "ParticipantName",
                "enrolmentstatus" => "EnrolmentStatus",
                "consentstatus" => "ConsentStatus",
                "currentlocation" => "CurrentLocation.Name",
                "enrolmentlocation" => "EnrolmentLocation.Name",
                "owner" => "Owner",
                "tenant" => "Tenant",
                "riskdue" => "RiskDue",
                _ => "Id"
            };

            var sortDirection = request.SortDirection.Equals("Ascending", StringComparison.OrdinalIgnoreCase)
                ? "ascending"
                : "descending";

            var data = await transformedQuery
                .AsNoTracking()
                .OrderBy($"{sortColumn} {sortDirection}")
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

            //May be at some point in future validate against columns of query result dataset
            RuleFor(r => r.OrderBy)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "OrderBy"));

        }
    }
}

