using Cfo.Cats.Application.Common.Enums;
using Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

namespace Cfo.Cats.Application.Features.PerformanceManagement.Providers;

public class ObjectiveEventsProvider : IPertinentEventProvider
{
    public async Task<IEnumerable<DipEventInformation>> GetEvents(string participantId, IApplicationDbContext context)
    {
        var query = from pp in context.PathwayPlans
            join obj in context.Objectives on pp.Id equals obj.PathwayPlanId
            where pp.ParticipantId == participantId
            select new
            {
                obj.Description,
                obj.Index,
                CreatedBy = obj.CreatedByUser!.DisplayName,
                obj.Created,
                obj.CompletedStatus,
                CompletedBy = obj.CompletedByUser!.DisplayName,
                obj.IsCompleted,
                obj.Justification,
                obj.Completed
            };

        var objectives = await query
            .AsNoTracking()
            .ToArrayAsync();

        List<DipEventInformation> events = [];

        events.AddRange(objectives.Select(o => new DipEventInformation
        {
            Contents =
            [
                o.Description
            ],
            Title = $"Objective {o.Index} created",
            ActionedBy = o.CreatedBy!,
            OccurredOn = o.Created!.Value,
            RecordedOn = o.Created!.Value,
            Colour = AppColour.Info,
            Icon = AppIcon.Objective,
        }));

        events.AddRange(objectives
            .Where(o => o.IsCompleted)
            .Select(o => new DipEventInformation
            {
                Contents =
                [
                    o.Description, o.Justification
                ],
                Title = $"Objective {o.Index} {(o.CompletedStatus! == CompletionStatus.Done ? "Completed" : "Abandoned")}",
                ActionedBy = o.CompletedBy!,
                OccurredOn = o.Completed!.Value,
                RecordedOn = o.Completed!.Value,
                Colour = o.CompletedStatus == CompletionStatus.Done ? AppColour.Success : AppColour.Warning,
                Icon = AppIcon.Objective,
            }));

        return events;

    }
}