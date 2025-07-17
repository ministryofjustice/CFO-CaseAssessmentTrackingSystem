using Cfo.Cats.Application.Common.Enums;
using Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

namespace Cfo.Cats.Application.Features.PerformanceManagement.Providers;

public class WingInductionPhaseEventsProvider : IPertinentEventProvider
{
    public async Task<IEnumerable<DipEventInformation>> GetEvents(string participantId, IApplicationDbContext context)
    {
        var query = from hi in context.WingInductions
                    join loc in context.Locations on hi.LocationId equals loc.Id
                    from phase in hi.Phases
                    join completedUser in context.Users on phase.CompletedBy equals completedUser.Id into completedUserGroup
                    from completedUser in completedUserGroup.DefaultIfEmpty()
                    where hi.ParticipantId == participantId
                    select new
                    {
                        InductionLocation = loc.Name,
                        phase.StartDate,
                        phase.CompletedDate,
                        phase.Status,
                        phase.AbandonReason,
                        phase.AbandonJustification,
                        CompletedByName = completedUser != null ? completedUser.DisplayName : string.Empty
                    };


        var results = await query.AsNoTracking().ToArrayAsync();

        return results.Select(ph => new DipEventInformation
        {
            Title = "Wing Induction Phase",
            ActionedBy = ph.CompletedByName,
            Contents = new[]
                        {
                            $"Induction Location: {ph.InductionLocation}"
                        }
                        .Concat(new[] { $"Started On: {ph.StartDate.ToShortDateString()}, Status: {ph.Status.Name}" })
                        .Concat(ph.Status == WingInductionPhaseStatus.Abandoned
                            ? new[] { $"Abandon Status: {ph.AbandonReason.Name}, Justification: {ph.AbandonJustification}" }
                            : Array.Empty<string>())
                        .Concat(ph.CompletedDate != null
                            ? new[] { $"Completed On: {ph.CompletedDate.Value.ToShortDateString()}" }
                            : Array.Empty<string>())
                        .ToArray(),
            OccurredOn = ph.StartDate,
            RecordedOn = ph.StartDate,
            Colour = AppColour.Success,
            Icon = AppIcon.WingInductionPhase
        });

    }
}
