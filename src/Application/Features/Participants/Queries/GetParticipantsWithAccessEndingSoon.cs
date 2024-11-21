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
            var backdatePeriod = DateTime.UtcNow.AddDays(-90);
            string ownerId = request.OwnerId ?? currentUser.UserId!;

            // Has the owner ever worked with anyone?
            if(await GetParticipantsOwnedByInPeriod(ownerId, backdatePeriod, cancellationToken) is not { Count: > 0 } participantsOwnedInLast90Days)
            {
                return [];
            }

            var unassignedParticipants = await unitOfWork.DbContext.ParticipantOwnershipHistories
                .Where(h => participantsOwnedInLast90Days.Contains(h.ParticipantId))
                .GroupBy(h => h.ParticipantId)
                .Select(g => new
                {
                    ParticipantId = g.Key,
                    CurrentOwnerId = g.OrderByDescending(oh => oh.From).First().OwnerId,
                    FromDate = g.OrderByDescending(oh => oh.From).First().From
                })
                .Where(x => x.CurrentOwnerId != ownerId)
                .ToDictionaryAsync(p => p.ParticipantId, cancellationToken);

            if(unassignedParticipants is not { Count: > 0 })
            {
                return [];
            }

            // At this point, our owner has been unassigned (in the last 90 days).
            // However, they may still have access to the participants' current location.

            var locations = await GetAccessibleLocations(ownerId, cancellationToken);

            var participantsWithAccessEndingSoon = await unitOfWork.DbContext.Participants
                .Where(p => unassignedParticipants.Select(x => x.Key).Contains(p.Id))
                .Where(p => locations.Contains(p.CurrentLocation.Id) == false)
                .ProjectTo<ParticipantDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            // Translate data
            return participantsWithAccessEndingSoon.Select(participant => new ParticipantWithAccessEndingSoonDto
            {
                Participant = participant, 
                LostAccessOn = unassignedParticipants[participant.Id].FromDate
            });
        }

        async Task<List<string>> GetParticipantsOwnedByInPeriod(string ownerId, DateTime backdatePeriod, CancellationToken cancellationToken)
        {
            var participants = await unitOfWork.DbContext.ParticipantOwnershipHistories
                .Where(h => h.OwnerId == ownerId && h.From >= backdatePeriod)
                .GroupBy(h => h.ParticipantId)
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
