using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetArchiveRecurrences
{
    [RequestAuthorize(Policy = SecurityPolicies.ArchiveRecurrences)]
    public class Query : IQuery<Result<ArchiveRecurrencesDto>>
    {
        public required DateTime StartDate { get; init; }
        public required DateTime EndDate { get; init; }
        public string? ContractId { get; init; }
        public int Threshold { get; init; } = 5;
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.EndDate)
                .Must((query, endDate) => endDate.Date >= query.StartDate.Date)
                .WithMessage("The end date cannot be before the start date.");

            RuleFor(x => x.Threshold)
                .InclusiveBetween(1, 20)
                .WithMessage("The recurrence threshold must be between 1 and 20.");
        }
    }

    public class Handler(
        IUnitOfWork unitOfWork,
        ILocationService locationService,
        ICurrentUserService currentUserService)
        : IQueryHandler<Query, Result<ArchiveRecurrencesDto>>
    {
        public async Task<Result<ArchiveRecurrencesDto>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var db = unitOfWork.DbContext;
            var query =
                from ac in db.ArchivedCases.AsNoTracking()
                join l in db.Locations.AsNoTracking() on ac.LocationId equals l.Id
                join c in db.Contracts.AsNoTracking() on ac.ContractId equals c.Id into contracts
                from c in contracts.DefaultIfEmpty()
                join u in db.Users.AsNoTracking() on ac.CreatedBy equals u.Id
                join p in db.Participants.AsNoTracking() on ac.ParticipantId equals p.Id
                where ac.From >= request.StartDate.Date
                      && ac.From < request.EndDate.Date.AddDays(1)
                select new
                {
                    ac,
                    l,
                    ContractName = c != null ? c.Description : null,
                    u
                };

            if (!currentUserService.IsInternalUser.GetValueOrDefault())
            {
                var visibleParticipantIds = GetVisibleParticipantIds();
                query = query.Where(x => visibleParticipantIds.Contains(x.ac.ParticipantId));
            }

            if (!string.IsNullOrWhiteSpace(request.ContractId))
            {
                var participantIdsInContract = GetParticipantIdsInContract(request.ContractId);
                query = query.Where(x => participantIdsInContract.Contains(x.ac.ParticipantId));
            }

            var qualifyingParticipantIds = query
                .GroupBy(x => x.ac.ParticipantId)
                .Where(group => group.Count() >= request.Threshold)
                .Select(group => group.Key);

            var episodes = await query
                .Where(x => qualifyingParticipantIds.Contains(x.ac.ParticipantId))
                .Select(x => new ArchiveEpisodeDto
                {
                    ParticipantId = x.ac.ParticipantId,
                    FirstName = x.ac.FirstName,
                    LastName = x.ac.LastName,
                    ContractId = x.ac.ContractId,
                    ContractName = x.ContractName,
                    LocationName = x.l.Name,
                    LocationType = x.ac.LocationType,
                    ArchiveReason = x.ac.ArchiveReason ?? "Unknown",
                    ArchiveAdditionalInfo = x.ac.AdditionalInfo,
                    UnarchiveReason = x.ac.UnarchiveReason,
                    UnarchiveAdditionalInfo = x.ac.UnarchiveAdditionalInfo,
                    From = x.ac.From,
                    To = x.ac.To,
                    CreatedBy = x.u.DisplayName
                })
                .OrderBy(x => x.From)
                .ToArrayAsync(cancellationToken);

            var participantIds = episodes
                .Select(x => x.ParticipantId)
                .Distinct()
                .ToArray();

            var currentContracts = await GetCurrentContractsAsync(participantIds, cancellationToken);

            return Result<ArchiveRecurrencesDto>.Success(
                new ArchiveRecurrencesDto(
                    episodes,
                    currentContracts));
        }

        private IQueryable<string> GetVisibleParticipantIds()
        {
            var db = unitOfWork.DbContext;
            var tenantId = currentUserService.TenantId;
            var userId = currentUserService.UserId;
            var cutoff = DateTime.Today.AddMonths(-3);
            var visibleLocationIds = locationService
                .GetVisibleLocations(tenantId!)
                .Select(x => x.Id)
                .ToArray();

            return db.Participants
                .AsNoTracking()
                .Where(p =>
                    db.ParticipantOwnershipHistories
                        .AsNoTracking()
                        .Where(oh => oh.ParticipantId == p.Id
                                     && (oh.To == null || oh.To >= cutoff))
                        .Select(oh => oh.TenantId)
                        .Any(t => t != null && t.StartsWith(tenantId!))
                    || db.ParticipantLocationHistories
                        .AsNoTracking()
                        .Where(lh => lh.ParticipantId == p.Id
                                     && (lh.To == null || lh.To >= cutoff))
                        .Select(lh => lh.LocationId)
                        .Any(id => visibleLocationIds.Contains(id))
                    || (userId != null && db.PRIs
                        .AsNoTracking()
                        .Where(pri => pri.ParticipantId == p.Id && pri.CompletedOn == null)
                        .Select(pri => pri.AssignedTo)
                        .FirstOrDefault() == userId))
                .Select(p => p.Id);
        }

        private IQueryable<string> GetParticipantIdsInContract(string contractId)
        {
            var db = unitOfWork.DbContext;
            return db.Participants
                .AsNoTracking()
                .Where(p =>
                    db.ParticipantOwnershipHistories
                        .AsNoTracking()
                        .Where(oh => oh.ParticipantId == p.Id
                                     && oh.To == null
                                     && oh.ContractId != null)
                        .Select(oh => oh.ContractId)
                        .Contains(contractId)
                    || !db.ParticipantOwnershipHistories
                        .AsNoTracking()
                        .Any(oh => oh.ParticipantId == p.Id
                                   && oh.To == null
                                   && oh.ContractId != null)
                        && p.CurrentLocation.Contract != null
                        && p.CurrentLocation.Contract.Id == contractId)
                .Select(p => p.Id);
        }

        private async Task<CurrentParticipantContractDto[]> GetCurrentContractsAsync(
            string[] participantIds,
            CancellationToken cancellationToken)
        {
            var db = unitOfWork.DbContext;
            var currentContracts = await db.Participants
                .AsNoTracking()
                .Where(p => participantIds.Contains(p.Id))
                .Select(p => new CurrentParticipantContractDto
                {
                    ParticipantId = p.Id,
                    OwnershipContractId = db.ParticipantOwnershipHistories
                        .AsNoTracking()
                        .Where(oh => oh.ParticipantId == p.Id
                                     && oh.To == null
                                     && oh.ContractId != null)
                        .Select(oh => oh.ContractId)
                        .FirstOrDefault(),
                    LocationContractId = p.CurrentLocation.Contract != null
                        ? p.CurrentLocation.Contract.Id
                        : null,
                    LocationContractName = p.CurrentLocation.Contract != null
                        ? p.CurrentLocation.Contract.Description
                        : null
                })
                .ToArrayAsync(cancellationToken);

            var contractIds = currentContracts
                .SelectMany(x => new[]
                {
                    x.OwnershipContractId,
                    x.LocationContractId
                })
                .Where(x => x is not null)
                .Select(x => x!)
                .Distinct()
                .ToArray();

            var contractNames = await db.Contracts
                .AsNoTracking()
                .Where(c => contractIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id, c => c.Description, cancellationToken);

            foreach (var currentContract in currentContracts)
            {
                var effectiveContractId = currentContract.OwnershipContractId
                                          ?? currentContract.LocationContractId;
                var effectiveContractName = effectiveContractId is not null
                    ? contractNames.GetValueOrDefault(effectiveContractId)
                      ?? currentContract.LocationContractName
                      ?? "Contract unknown"
                    : "Contract unknown";

                currentContract.ContractId = effectiveContractId;
                currentContract.ContractName = effectiveContractName;
            }

            return currentContracts;
        }
    }

}
