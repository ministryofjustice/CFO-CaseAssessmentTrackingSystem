using Cfo.Cats.Application.Common.Enums;
using Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

namespace Cfo.Cats.Application.Features.PerformanceManagement.Providers;

public class ActivityEventsProvider : IPertinentEventProvider
{
    public async Task<IEnumerable<DipEventInformation>> GetEvents(string participantId, IApplicationDbContext context)
    {

        var query = from a in context.Activities
            join u in context.Users on a.CreatedBy equals u.Id
            join ot in context.ObjectiveTasks on a.TaskId equals ot.Id
            join o in context.Objectives on ot.ObjectiveId equals o.Id
            where a.ParticipantId == participantId
            select new
            {
                Created = a.Created!.Value,
                CreatedBy = u.DisplayName,
                Type = a.Type.Name,
                a.CommencedOn,
                Category = a.Category.Name,
                a.AdditionalInformation,
                TookPlaceAt = a.TookPlaceAtLocation.Name,
                Classification = a.Definition.Classification.Name,
                TaskIndex = ot.Index,
                ObjectiveIndex = o.Index
            };

        var results = await query
            .AsNoTracking()
            .ToArrayAsync();

        return results.Select(r => new DipEventInformation
        {
            OccurredOn = r.CommencedOn,
            ActionedBy = r.CreatedBy,
            RecordedOn = r.Created,
            Title = $"{r.Classification} added",
            Colour = AppColour.Success,
            Contents =
            [
                $"{r.Type} ({r.Category}) took place in {r.TookPlaceAt}.",
                r.AdditionalInformation ?? string.Empty,
                $"This is attached to task {r.TaskIndex} on objective {r.ObjectiveIndex}"
            ],
            Icon = AppIcon.Activity,
        });

    }
}