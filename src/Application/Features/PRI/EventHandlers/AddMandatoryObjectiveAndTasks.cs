using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.PRI.EventHandlers;

public class AddMandatoryObjectiveAndTasks(IUnitOfWork unitOfWork) : INotificationHandler<PRICreatedDomainEvent>
{
    public async Task Handle(PRICreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var pathwayPlan = await unitOfWork.DbContext.PathwayPlans
            .OrderByDescending(p => p.Created)
            .FirstOrDefaultAsync(p => p.ParticipantId == notification.ParticipantId, cancellationToken);

        if (pathwayPlan is null)
        {
            throw new NotFoundException($"No Pathway Plan found for {notification.ParticipantId}");
        }

        // Used exclusively to give indication of which month a task is due
        var monthOfRelease = new DateTime(
            year: notification.ExpectedReleaseDate.Year,
            month: notification.ExpectedReleaseDate.Month,
            day: 1);

        var objective = Objective.Create($"Through the Gate support: {notification.ExpectedReleaseDate.ToShortDateString()}", pathwayPlan.Id, isMandatory: true);

        List<ObjectiveTask> tasks =
        [
            ObjectiveTask.Create("Community Support Worker to contact the participant prior to release", monthOfRelease, isMandatory: true),
            ObjectiveTask.Create("Community Support Worker to meet the participant after release to ensure continued support", monthOfRelease, isMandatory: true)
        ];

        tasks.ForEach(task => objective.AddTask(task));

        pathwayPlan.AddObjective(objective);
    }
}
