using Cfo.Cats.Application.Common.Enums;
using Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

namespace Cfo.Cats.Application.Features.PerformanceManagement.Providers;

public class ObjectiveTaskEventsProvider : IPertinentEventProvider
{

    public async Task<IEnumerable<DipEventInformation>> GetEvents(string participantId, IApplicationDbContext context)
    {
        var query = from pp in context.PathwayPlans
            join obj in context.Objectives on pp.Id equals obj.PathwayPlanId
            join task in context.ObjectiveTasks on obj.Id equals task.ObjectiveId
            join creationUser in context.Users on task.CreatedBy equals creationUser.Id
            join completionUser in context.Users on task.CompletedBy equals completionUser.Id into completionUsers
            from completionUser in completionUsers.DefaultIfEmpty()
            where pp.ParticipantId == participantId
            select new
            {
                task.Description,
                CreatedBy = creationUser.DisplayName,
                task.Created,
                task.Completed,
                CompletedBy = completionUser != null ? completionUser.DisplayName : null,
                task.CompletedStatus,
                task.Justification,
                task.Index,
                ObjectiveIndex = obj.Index
            };

        var results = await query.AsNoTracking().ToArrayAsync();

        return results.Select(t => new DipEventInformation
        {
            Title = $"Task {t.Index} created on objective {t.ObjectiveIndex}",
            Contents = [
                t.Description
            ],
            ActionedBy = t.CreatedBy,
            OccurredOn = t.Created!.Value,
            RecordedOn = t.Created!.Value,
            Icon = AppIcon.Task,
        })
            .Union(
            results.Where(t => t.CompletedBy != null)
                .Select(t => new DipEventInformation
                {
                    Title = $"Task {t.Index} {(t.CompletedStatus == CompletionStatus.Done ? "Completed" : "Abandoned")} on objective {t.ObjectiveIndex}",
                    Contents = [
                    t.Description, 
                    t.Justification],
                    ActionedBy = t.CompletedBy,
                    OccurredOn = t.Completed!.Value,
                    RecordedOn = t.Completed!.Value,
                    Icon = AppIcon.Task,
                })

            );



    }
}
