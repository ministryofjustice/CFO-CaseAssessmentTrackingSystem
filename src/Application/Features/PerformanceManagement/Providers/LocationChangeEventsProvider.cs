using Cfo.Cats.Application.Common.Enums;
using Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

namespace Cfo.Cats.Application.Features.PerformanceManagement.Providers;

public class LocationChangeEventProvider : IPertinentEventProvider
{
    public async Task<IEnumerable<DipEventInformation>> GetEvents(string participantId, IApplicationDbContext context)
    {
        var query = (from ph in context.ParticipantLocationHistories
            join l in context.Locations on ph.LocationId equals l.Id
            join u in context.Users on ph.CreatedBy equals u.Id into userJoin
            from user in userJoin.DefaultIfEmpty()
            where ph.ParticipantId == participantId
            select new
            {
                ActionedBy = user.DisplayName,
                Location = l.Name,
                OccuredOn = ph.Created!
            }).AsNoTracking();

        var results = await query.ToArrayAsync();

        return results
            // We are not interested in the first "change" 
            // as this is the initial registration
            .Skip(1)
            .Select(r => new DipEventInformation
        {
            Title = "Location changed",
            ActionedBy = r.ActionedBy ?? "System Change",
            Contents = [
                $"Registered at {r.Location}", 
                "This is when CATS knew about the movement change",
                "This should be treated with caution as delays in the feed can give a misrepresentation"
            ],
            OccurredOn = r.OccuredOn!.Value,
            RecordedOn = r.OccuredOn!.Value,
            Icon = AppIcon.Location,
        });
    }
}