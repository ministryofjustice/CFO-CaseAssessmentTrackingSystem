using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class UnassignedCasesWithPagination
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : PaginationFilter, IRequest<Result<PaginatedData<UnassignedCaseDto>>>
    {
        /// <summary>
        /// The currently logged in user
        /// </summary>
        public UserProfile? CurrentUser { get; set; }

        /// <summary>
        /// Filter by enrolment status
        /// </summary>
        public int? EnrolmentStatus { get; set; }

        /// <summary>
        /// Filter by location IDs
        /// </summary>
        public int[] Locations { get; set; } = [];
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<PaginatedData<UnassignedCaseDto>>>
    {
        public async Task<Result<PaginatedData<UnassignedCaseDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            // Get visible locations for the current user's tenant
            var visibleLocationIds = context.Locations
                .Where(l => l.Tenants.Any(t => t.Id.StartsWith(request.CurrentUser!.TenantId!)))
                .Select(l => l.Id)
                .ToList();

            // Get participants without owners at locations visible to the current user's tenant
            var query = from p in context.Participants
                        where p.OwnerId == null
                              && (visibleLocationIds.Contains(p.CurrentLocation.Id) 
                                  || (p.EnrolmentLocation != null && visibleLocationIds.Contains(p.EnrolmentLocation.Id)))
                        select p;

            // Apply keyword search
            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                if (request.Keyword.Split(" ") is { Length: 2 } segments)
                {
                    query = query.Where(p => p.FirstName.Contains(segments[0]) && p.LastName.Contains(segments[1]));
                }
                else
                {
                    query = query.Where(p => p.FirstName.Contains(request.Keyword)
                                || p.LastName!.Contains(request.Keyword)
                                || p.Id.Contains(request.Keyword)
                                || p.ExternalIdentifiers.Any(ei => ei.Value.Contains(request.Keyword)));
                }
            }

            // Apply enrolment status filter
            if (request.EnrolmentStatus.HasValue)
            {
                query = query.Where(p => p.EnrolmentStatus == request.EnrolmentStatus.Value);
            }

            // Apply location filter
            if (request.Locations.Length > 0)
            {
                query = query.Where(p => request.Locations.Contains(p.CurrentLocation.Id)
                                         || (p.EnrolmentLocation != null && request.Locations.Contains(p.EnrolmentLocation.Id)));
            }

            var count = await query.AsNoTracking().CountAsync(cancellationToken);

            // Apply sorting and pagination first
            var sortColumn = request.OrderBy.Trim().ToLowerInvariant() switch
            {
                "id" => "Id",
                "firstname" => "FirstName",
                "lastname" => "LastName",
                "enrolmentstatus" => "EnrolmentStatus",
                "consentstatus" => "ConsentStatus",
                "currentlocation" => "CurrentLocation.Name",
                "currentlocation.name" => "CurrentLocation.Name",
                "lastmodified" => "LastModified",
                _ => "LastModified"
            };

            var sortDirection = request.SortDirection.Equals("Ascending", StringComparison.OrdinalIgnoreCase)
                ? "ascending"
                : "descending";

            var paginatedParticipants = await query
                .AsNoTracking()
                .OrderBy($"{sortColumn} {sortDirection}")
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new
                {
                    p.Id,
                    p.FirstName,
                    p.LastName,
                    p.EnrolmentStatus,
                    p.ConsentStatus,
                    p.LastModified,
                    CurrentLocationId = p.CurrentLocation!.Id,
                    CurrentLocationName = p.CurrentLocation.Name,
                    CurrentLocationGenderProvisionValue = p.CurrentLocation.GenderProvision.Value,
                    CurrentLocationLocationTypeValue = p.CurrentLocation.LocationType.Value,
                    CurrentLocationContractDescription = p.CurrentLocation.Contract!.Description,
                    EnrolmentLocationId = p.EnrolmentLocation != null ? (int?)p.EnrolmentLocation.Id : null,
                    EnrolmentLocationName = p.EnrolmentLocation != null ? p.EnrolmentLocation.Name : null,
                    EnrolmentLocationGenderProvisionValue = p.EnrolmentLocation != null ? (int?)p.EnrolmentLocation.GenderProvision.Value : null,
                    EnrolmentLocationLocationTypeValue = p.EnrolmentLocation != null ? (int?)p.EnrolmentLocation.LocationType.Value : null,
                    EnrolmentLocationContractDescription = p.EnrolmentLocation != null ? p.EnrolmentLocation.Contract!.Description : null
                })
                .ToListAsync(cancellationToken);

            // Get location IDs to fetch tenant info
            var locationIds = paginatedParticipants.Select(p => p.CurrentLocationId).Distinct().ToList();

            // Fetch tenant info for these locations
            var locationTenants = await (from l in context.Locations
                                        where locationIds.Contains(l.Id)
                                        from t in l.Tenants
                                        select new LocationTenantDto
                                        {
                                            LocationId = l.Id,
                                            TenantId = t.Id,
                                            TenantName = t.Name
                                        })
                                        .ToListAsync(cancellationToken);

            var locationTenantMap = locationTenants
                .GroupBy(lt => lt.LocationId)
                .ToDictionary(g => g.Key, g => g.OrderBy(t => t.TenantId).First());

            // Get incoming transfers
            var participantIds = paginatedParticipants.Select(p => p.Id).ToList();
            var incomingTransfers = await context.ParticipantIncomingTransferQueue
                .Where(t => !t.Completed && participantIds.Contains(t.ParticipantId))
                .Select(t => new { t.ParticipantId, t.Id })
                .ToListAsync(cancellationToken);

            var transferMap = incomingTransfers.ToDictionary(t => t.ParticipantId, t => t.Id);

            // Map to DTOs
            var data = paginatedParticipants.Select(p =>
            {
                var tenantInfo = locationTenantMap.GetValueOrDefault(p.CurrentLocationId);
                var transferId = transferMap.GetValueOrDefault(p.Id);
                
                return new UnassignedCaseDto
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    EnrolmentStatus = p.EnrolmentStatus!,
                    ConsentStatus = p.ConsentStatus!,
                    CurrentLocation = new LocationDto
                    {
                        Id = p.CurrentLocationId,
                        Name = p.CurrentLocationName,
                        GenderProvision = GenderProvision.FromValue(p.CurrentLocationGenderProvisionValue),
                        LocationType = LocationType.FromValue(p.CurrentLocationLocationTypeValue),
                        ContractName = p.CurrentLocationContractDescription
                    },
                    EnrolmentLocation = p.EnrolmentLocationId.HasValue
                        ? new LocationDto
                        {
                            Id = p.EnrolmentLocationId.Value,
                            Name = p.EnrolmentLocationName!,
                            GenderProvision = GenderProvision.FromValue(p.EnrolmentLocationGenderProvisionValue!.Value),
                            LocationType = LocationType.FromValue(p.EnrolmentLocationLocationTypeValue!.Value),
                            ContractName = p.EnrolmentLocationContractDescription!
                        }
                        : null,
                    TenantId = tenantInfo?.TenantId ?? string.Empty,
                    TenantName = tenantInfo?.TenantName ?? string.Empty,
                    LastModified = p.LastModified,
                    HasIncomingTransfer = transferId != default,
                    IncomingTransferId = transferId != default ? transferId : null
                };
            }).ToList();

            return new PaginatedData<UnassignedCaseDto>(data, count, request.PageNumber, request.PageSize);
        }
    }

    internal class LocationTenantDto
    {
        public int LocationId { get; set; }
        public string TenantId { get; set; } = default!;
        public string TenantName { get; set; } = default!;
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.CurrentUser)
                .NotNull()
                .WithMessage("Current user is required");

            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page number must be greater than or equal to 1");

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(100)
                .WithMessage("Page size must be between 1 and 100");
        }
    }
}
