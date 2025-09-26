using Cfo.Cats.Application.Features.Activities.IntegrationEvents;
using Cfo.Cats.Application.Features.Assessments.IntegrationEvents;
using Cfo.Cats.Application.Features.Inductions.IntegrationEvents;
using Cfo.Cats.Application.Features.Participants.IntegrationEvents;
using Cfo.Cats.Application.Features.PRIs.IntegrationEvents;
using Rebus.Bus;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;

public class RaisePaymentsAfterApprovalConsumer(IUnitOfWork unitOfWork, IBus bus) : IHandleMessages<ParticipantTransitionedIntegrationEvent>
{
    public async Task Handle(ParticipantTransitionedIntegrationEvent context)
    {
        if (context.To != EnrolmentStatus.ApprovedStatus.Name)
        {
            return;
        }

        if (await IsFirstApproval(context.ParticipantId))
        {
            List<object> events = [];

            var wings = await unitOfWork.DbContext.WingInductions
                .Where(e => e.ParticipantId == context.ParticipantId)
                .Select(e => e.Id)
                .ToArrayAsync();

            var hubs = await unitOfWork.DbContext.HubInductions
                .Where(e => e.ParticipantId == context.ParticipantId)
                .Select(e => e.Id)
                .ToArrayAsync();

            var activities = await unitOfWork.DbContext.Activities.Where(e => e.ParticipantId == context.ParticipantId)
                .Where(e => e.CompletedOn != null && e.Status == ActivityStatus.ApprovedStatus)
                .Select(e => new { e.Id, e.CompletedOn })
                .ToArrayAsync();

            var pris = await unitOfWork.DbContext.PRIs.Where(e => e.ParticipantId == context.ParticipantId)
                .Where(e => e.AssignedTo != null)
                .Select(e => e)
                .ToArrayAsync();

            var mandatoryTasks = (await unitOfWork.DbContext.PathwayPlans
                .AsNoTracking()
                .SelectMany(p => p.Objectives.Where(o => pris.Select(p => p.ObjectiveId).Contains(o.Id)))
                .SelectMany(o => o.Tasks.Where(t => t.Completed != null && t.Index == 2))
                .ToArrayAsync())
                .Select(task => new { task, PRI = pris.First(p => p.ObjectiveId == task.ObjectiveId) });

            var reassessments = await unitOfWork.DbContext.ParticipantAssessments
                .Where(a => a.ParticipantId == context.ParticipantId && a.Completed != null)
                .Select(p => new { p.Id, p.ParticipantId, p.Completed })
                .OrderBy(a => a.Completed)
                .Skip(1) // Ignore initial assessment
                .ToArrayAsync();

            events.AddRange(wings.Select(wi => new WingInductionCreatedIntegrationEvent(wi, DateTime.UtcNow)));
            events.AddRange(hubs.Select(hi => new HubInductionCreatedIntegrationEvent(hi, DateTime.UtcNow)));
            events.AddRange(activities.Select(e => new ActivityApprovedIntegrationEvent(e.Id, e.CompletedOn!.Value)));
            events.AddRange(pris.Select(e => new PRIAssignedIntegrationEvent(e.Id, e.MeetingAttendedOn.ToDateTime(TimeOnly.MinValue))));
            events.AddRange(mandatoryTasks.Select(task => new PRIThroughTheGateCompletedIntegrationEvent(task.PRI.Id)));
            events.AddRange(reassessments.Select(r => new AssessmentScoredIntegrationEvent(r.Id, r.ParticipantId, r.Completed!.Value)));

            foreach (var message in events)
            {
                await bus.Publish(message);
            }
        }
    }

    private async Task<bool> IsFirstApproval(string participantId)
    {
        var history = await unitOfWork.DbContext.ParticipantEnrolmentHistories
            .Where(e => e.ParticipantId == participantId)
            .ToArrayAsync();

        return history.Count(e => e.EnrolmentStatus == EnrolmentStatus.ApprovedStatus) <= 1;
    }
}