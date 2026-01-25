using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

namespace Cfo.Cats.Application.Features.PerformanceManagement.Providers;

public class ParticipantReassignedEventProvider : IPertinentEventProvider
{
    public async Task<IEnumerable<DipEventInformation>> GetEvents(string participantId, IApplicationDbContext context)
    {
        var query = from ph in context.ParticipantOwnershipHistories
            join u in context.Users on ph.CreatedBy equals u.Id into creationJoin
            from creationUser in creationJoin.DefaultIfEmpty()
            join u in context.Users on ph.OwnerId equals u.Id into owningJoin
            from owningUser in owningJoin.DefaultIfEmpty()
                    where ph.ParticipantId == participantId
            select new
            {
                ph.Created,
                CreatedBy = creationUser.DisplayName,
                Owner = owningUser.DisplayName
            };

        var results = await query.AsNoTracking()
            .ToArrayAsync();

        var entries = results
            .Skip(1)
            .Select((current, i) => new DipEventInformation
            {
                Title = "Case Ownership Changed",
                OccurredOn = current.Created!.Value.Date,
                RecordedOn = current.Created!.Value,
                ActionedBy = current.CreatedBy ?? "System",
                Icon = AppIcon.Reassignment,
                Contents =
                [
                    current.Owner == null
                        ? "Participant has been unassigned"
                        : $"Participant reassigned to {current.Owner}",
                    results[i].Owner == null
                        ? "From no assignee"
                        : $"From {results[i].Owner}"
                ],
                Colour = AppColour.Primary
            })
            .ToList();

        return entries;

    }
}