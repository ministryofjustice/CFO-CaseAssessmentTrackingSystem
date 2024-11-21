using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetParticipantsWithAccessEndingSoon
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Query : IRequest<IEnumerable<ParticipantWithAccessEndingSoonDto>>
    {
        /// <summary>
        /// Participant Owner. Current user will be used if not provided.
        /// </summary>
        public string? OwnerId { get; set; }
    }

    // Todo: move this dto
    public record ParticipantWithAccessEndingSoonDto
    {
        public required DateTime LostAccessOn { get; set; }
        public DateTime HasAccessTo => LostAccessOn.AddDays(90);
        public required ParticipantDto Participant { get; set; }
    }

    class Handler(
        IUnitOfWork unitOfWork, 
        ICurrentUserService currentUser,
        ILocationService locationService,
        IMapper mapper) : IRequestHandler<Query, IEnumerable<ParticipantWithAccessEndingSoonDto>>
    {
        public async Task<IEnumerable<ParticipantWithAccessEndingSoonDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var lastNinetyDays = DateTime.UtcNow.AddDays(-90);
            string ownerId = request.OwnerId ?? currentUser.UserId!;

            var participantsWhoLeftOwnershipInLastNinetyDays = await GetParticipantsWhoLeftOwnershipInPeriod(ownerId, lastNinetyDays, cancellationToken);

            // Has the owner ever worked with anyone?
            if (participantsWhoLeftOwnershipInLastNinetyDays.Any() is false)
            {
                return [];
            }

            // Retrieve a complete history of participant ownership (includes the other owners)
            var completeParticipantsOwnerHistory = await unitOfWork.DbContext.ParticipantOwnershipHistories
                .Where(h => participantsWhoLeftOwnershipInLastNinetyDays.Contains(h.ParticipantId))
                .GroupBy(h => h.ParticipantId)
                .Select(g => new
                {
                    ParticipantId = g.Key,
                    OwnershipHistory = g.OrderByDescending(h => h.From).ToList()
                }).ToListAsync(cancellationToken);

            // Work out which participants have been unassigned, and when the ownership changed
            var participantsUnassignedInLastNinetyDays = completeParticipantsOwnerHistory
                .Select(g => new
                {
                    g.ParticipantId,
                    CurrentOwnerId = g.OwnershipHistory.First().OwnerId,
                    PreviousOwnerHasAccessTo = g.OwnershipHistory.First(h => h.OwnerId == ownerId).To
                })
                .Where(p => p.CurrentOwnerId != ownerId)
                .ToDictionary(p => p.ParticipantId);

            if (participantsUnassignedInLastNinetyDays.Any() is false)
            {
                return [];
            }

            // At this point, our owner has been unassigned at some point within the last 90 days.
            // However, they may still have access to the participants' current location.
            var locations = await GetAccessibleLocations(ownerId, cancellationToken);

            var participantsWithAccessEndingSoon = await unitOfWork.DbContext.Participants
                .Where(p => participantsUnassignedInLastNinetyDays.Select(x => x.Key).Contains(p.Id))
                .Where(p => locations.Contains(p.CurrentLocation.Id) == false)
                .ProjectTo<ParticipantDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            // Translate data
            return participantsWithAccessEndingSoon.Select(participant => new ParticipantWithAccessEndingSoonDto
            {
                Participant = participant, 
                LostAccessOn = participantsUnassignedInLastNinetyDays[participant.Id].PreviousOwnerHasAccessTo!.Value
            });
        }

        async Task<List<string>> GetParticipantsWhoLeftOwnershipInPeriod(string ownerId, DateTime backdatePeriod, CancellationToken cancellationToken)
        {
            var participants = await unitOfWork.DbContext.ParticipantOwnershipHistories
                .Where(h => h.OwnerId == ownerId && h.To >= backdatePeriod)
                .GroupBy(h => h.ParticipantId)
                .Where(h => h.OrderByDescending(x => x.From).First().OwnerId != ownerId) // Ensures the Owner does not 
                .Select(h => h.Key)
                .ToListAsync(cancellationToken);

            return participants;
        }

        async Task<IEnumerable<int>> GetAccessibleLocations(string ownerId, CancellationToken cancellationToken)
        {
            var owner = await unitOfWork.DbContext.Users
                .FirstAsync(u => u.Id == ownerId, cancellationToken);

            return locationService.GetVisibleLocations(owner.TenantId!)
                .Select(l => l.Id);
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
        }

    }
}
