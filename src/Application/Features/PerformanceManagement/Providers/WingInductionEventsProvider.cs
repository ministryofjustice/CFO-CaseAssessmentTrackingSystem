using Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

namespace Cfo.Cats.Application.Features.PerformanceManagement.Providers;

public class WingInductionEventsProvider : IPertinentEventProvider
{
    public async Task<IEnumerable<DipEventInformation>> GetEvents(string participantId, IApplicationDbContext context)
    {
        var query = from hi in context.WingInductions
                    join u in context.Users on hi.CreatedBy equals u.Id
                    join l in context.Locations on hi.LocationId equals l.Id
                    where hi.ParticipantId == participantId
                    select new
                    {
                        u.DisplayName,
                        hi.Created,
                        l.Name,
                        hi.InductionDate
                    };

        var results = await query.AsNoTracking().ToArrayAsync();

        return results.Select(r => new DipEventInformation
        {
            Title = "Wing Induction Created",
            ActionedBy = r.DisplayName,
            Contents =
            [
                  $"Induction Location: {r.Name}"
            ],
            OccurredOn = r.InductionDate,
            RecordedOn = r.Created!.Value,
            Colour = AppColour.Success,
            Icon = AppIcon.HubInduction,
        });

    }
}
