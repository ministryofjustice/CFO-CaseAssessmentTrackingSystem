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
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : PaginationFilter, IQuery<Result<PaginatedData<ParticipantPaginationDto>>>
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
        public RecentParticipantFilter RecentAction { get; set; } = RecentParticipantFilter.All;

        /// <summary>
        /// How the list should be grouped (e.g. by assignee). Defaults to no grouping.
        /// </summary>
        public ParticipantGroupBy GroupBy { get; set; } = ParticipantGroupBy.None;
        }

    public class Handler(IUnitOfWork unitOfWork) : IQueryHandler<Query, Result<PaginatedData<ParticipantPaginationDto>>>
    {
        /// <summary>
        /// Builds the filtered participant query shared by both the paginated list and the
        /// group-count summary, ensuring identical filtering semantics between the two.
        /// </summary>
        internal static IQueryable<Domain.Entities.Participants.Participant> ApplyFilter(IApplicationDbContext context, Query request)
        {
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

            DateTime? recentlyAssignedCutoff = request.RecentAction switch
            {
                RecentParticipantFilter.AssignedLast10Days => DateTime.UtcNow.Date.AddDays(-10),
                RecentParticipantFilter.AssignedLast30Days => DateTime.UtcNow.Date.AddDays(-30),
                _ => null
            };

            DateTime? recentlyVisitedCutoff = request.RecentAction switch
            {
                RecentParticipantFilter.VisitedLast7Days => DateTime.UtcNow.Date.AddDays(-7),
                _ => null
            };

            if (recentlyAssignedCutoff.HasValue)
            {
                // Filter participants who have ownership history within the date range and are currently assigned
                var participantIdsWithRecentOwnership = context.ParticipantOwnershipHistories
                    .Where(oh => oh.OwnerId == request.CurrentUser!.UserId
                                 && oh.From >= recentlyAssignedCutoff.Value
                                 && oh.To == null)
                    .Select(oh => oh.ParticipantId)
                    .Distinct();

                query = query.Where(p => participantIdsWithRecentOwnership.Contains(p.Id));
            }

            if (recentlyVisitedCutoff.HasValue)
            {
                // Filter participants who have access audit trail within the date range for currently logged in user
                var participantIdsWithAccessAuditTrail = context.AccessAuditTrails
                    .Where(oh => oh.UserId == request.CurrentUser!.UserId
                                 && oh.AccessDate >= recentlyVisitedCutoff.Value)
                    .Select(oh => oh.ParticipantId)
                    .Distinct();

                query = query.Where(p => participantIdsWithAccessAuditTrail.Contains(p.Id));
            }

            return query;
        }

        public async Task<Result<PaginatedData<ParticipantPaginationDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var query = ApplyFilter(context, request);

            var count = await query.AsNoTracking().CountAsync(cancellationToken);

            var transformedQuery = ProjectToDto(context, query, request);

            var orderExpression = BuildOrderExpression(request, includeGroup: true);

            var data = await transformedQuery
                .AsNoTracking()
                .OrderBy(orderExpression)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedData<ParticipantPaginationDto>(data, count, request.PageNumber, request.PageSize);

        }

        /// <summary>
        /// Projects a filtered participant query into <see cref="ParticipantPaginationDto"/>, including
        /// the optional "assigned on" / "accessed on" history joins required by the recent-action filters.
        /// Shared by the flat paginated list and the grouped view so projection stays identical.
        /// </summary>
        internal static IQueryable<ParticipantPaginationDto> ProjectToDto(
            IApplicationDbContext context,
            IQueryable<Domain.Entities.Participants.Participant> query,
            Query request)
        {
            // The recent-action filter decides which history table (if any) we join to surface the
            // "assigned on" / "accessed on" dates. The cutoff dates themselves are applied in ApplyFilter.
            var includeAssignedOn = request.RecentAction is RecentParticipantFilter.AssignedLast10Days
                or RecentParticipantFilter.AssignedLast30Days;

            var includeAccessedOn = request.RecentAction is RecentParticipantFilter.VisitedLast7Days;

            // Only join to the extra history tables when the selected filter needs those dates.
            var transformedSource = includeAssignedOn
                ? from p in query
                  join oh in (
                      from h in context.ParticipantOwnershipHistories
                      where h.OwnerId == request.CurrentUser!.UserId
                            && h.To == null
                      group h by h.ParticipantId into g
                      select new
                      {
                          ParticipantId = g.Key,
                          MostRecentFrom = g.Max(x => x.From)
                      }
                  ) on p.Id equals oh.ParticipantId into ownershipGroup
                  from ownership in ownershipGroup.DefaultIfEmpty()
                  select new
                  {
                      AssignedOn = ownership != null ? ownership.MostRecentFrom : (DateTime?)null,
                      AccessedOn = (DateTime?)null,
                      Participant = p
                  }
                : includeAccessedOn
                ? from p in query
                  join oh in (
                      from h in context.AccessAuditTrails
                      where h.UserId == request.CurrentUser!.UserId
                      group h by h.ParticipantId into g
                      select new
                      {
                          ParticipantId = g.Key,
                          MostRecentAccess = g.Max(x => x.AccessDate)
                      }
                  ) on p.Id equals oh.ParticipantId into visitedGroup
                  from visited in visitedGroup.DefaultIfEmpty()
                  select new
                  {
                      AssignedOn = (DateTime?)null,
                      AccessedOn = visited != null ? visited.MostRecentAccess : (DateTime?)null,
                      Participant = p
                  }
                : from p in query
                  select new
                  {
                      AssignedOn = (DateTime?)null,
                      AccessedOn = (DateTime?)null,
                      Participant = p
                  };

            return from item in transformedSource
                select new ParticipantPaginationDto()
                {
                    AssignedOn = item.AssignedOn,
                    AccessedOn = item.AccessedOn,
                    EnrolmentStatus = item.Participant.EnrolmentStatus!,
                    Owner = item.Participant.Owner!.DisplayName!,
                    ConsentStatus = item.Participant.ConsentStatus!,
                    CurrentLocation = new LocationDto
                    {
                        Id = item.Participant.CurrentLocation.Id,
                        Name = item.Participant.CurrentLocation.Name,
                        GenderProvision = item.Participant.CurrentLocation.GenderProvision,
                        LocationType = item.Participant.CurrentLocation.LocationType,
                        ContractName = item.Participant.CurrentLocation.Contract!.Description
                    },
                    Id = item.Participant.Id,
                    EnrolmentLocation = item.Participant.EnrolmentLocation == null
                        ? null
                        : new LocationDto
                        {
                            Name = item.Participant.EnrolmentLocation.Name,
                            GenderProvision = item.Participant.EnrolmentLocation.GenderProvision,
                            LocationType = item.Participant.EnrolmentLocation.LocationType,
                            Id = item.Participant.EnrolmentLocation.Id,
                            ContractName = item.Participant.EnrolmentLocation.Contract!.Description
                        },
                    FirstName = item.Participant.FirstName,
                    LastName = item.Participant.LastName,
                    RiskDue = item.Participant.RiskDue,
                    RiskDueReason = item.Participant.RiskDueReason!,
                    Tenant = item.Participant.Owner!.TenantName!,
                    Labels = (
                            from pl in context.ParticipantLabels
                            where EF.Property<string>(pl, "_participantId") == item.Participant.Id
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
                    ConsentGranted = item.Participant.DateOfFirstConsent
                };
        }

        internal static string GetSortColumn(string orderBy)
            => orderBy.Trim().ToLowerInvariant() switch
            {
                "id" => "Id",
                "firstname" => "FirstName",
                "enrolmentstatus" => "EnrolmentStatus",
                "consentstatus" => "ConsentStatus",
                "currentlocation" => "CurrentLocation.Name",
                "enrolmentlocation" => "EnrolmentLocation.Name",
                "owner" => "Owner",
                "tenant" => "Tenant",
                "riskdue" => "RiskDue",
                "lastname" => "LastName",
                "assignedon" => "AssignedOn",
                "accessedon" => "AccessedOn",
                _ => "Id"
            };

        /// <summary>
        /// Builds the dynamic LINQ order expression. When <paramref name="includeGroup"/> is true and a
        /// grouping is active, the group key leads the ordering so grouped rows stay contiguous (used by
        /// the flat/grouped export). The grouped UI fetches a single group at a time and does not need it.
        /// </summary>
        internal static string BuildOrderExpression(Query request, bool includeGroup)
        {
            var sortColumn = GetSortColumn(request.OrderBy);

            var sortDirection = request.SortDirection.Equals("Ascending", StringComparison.OrdinalIgnoreCase)
                ? "ascending"
                : "descending";

            var orderExpression = $"{sortColumn} {sortDirection}";

            if (!includeGroup)
            {
                return orderExpression;
            }

            var groupColumn = request.GroupBy switch
            {
                ParticipantGroupBy.Assignee => "Owner",
                ParticipantGroupBy.Tenant => "Tenant",
                _ => null
            };

            if (groupColumn is not null)
            {
                orderExpression = string.Equals(groupColumn, sortColumn, StringComparison.OrdinalIgnoreCase)
                    ? $"{groupColumn} {sortDirection}"
                    : $"{groupColumn} ascending, {orderExpression}";
            }

            return orderExpression;
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

