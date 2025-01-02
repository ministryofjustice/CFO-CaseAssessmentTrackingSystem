using Cfo.Cats.Application.Features.Activities.IntegrationEvents;
using Cfo.Cats.Application.Features.Inductions.IntegrationEvents;
using Cfo.Cats.Application.Features.Participants.IntegrationEvents;
using MassTransit;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;

public class RaisePaymentsAfterApprovalConsumer(IUnitOfWork unitOfWork) : IConsumer<ParticipantTransitionedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ParticipantTransitionedIntegrationEvent> context)
    {
        if (context.Message.To != EnrolmentStatus.ApprovedStatus.Name)
        {
            return;
        }

        if (await IsFirstApproval(context.Message.ParticipantId))
        {
            List<object> events = [];

            var wings = await unitOfWork.DbContext.WingInductions
                .Where(e => e.ParticipantId == context.Message.ParticipantId)
                .Select(e => e.Id)
                .ToArrayAsync();

            var hubs = await unitOfWork.DbContext.HubInductions
                .Where(e => e.ParticipantId == context.Message.ParticipantId)
                .Select(e => e.Id)
                .ToArrayAsync();

            var activities = await unitOfWork.DbContext.Activities.Where(e => e.ParticipantId == context.Message.ParticipantId)
                .Where(e => e.ApprovedOn != null)
                .Select(e => new { e.Id, e.ApprovedOn })
                .ToArrayAsync();

            events.AddRange(wings.Select(wi => new WingInductionCreatedIntegrationEvent(wi, DateTime.UtcNow)));
            events.AddRange(hubs.Select(hi => new HubInductionCreatedIntegrationEvent(hi, DateTime.UtcNow)));
            events.AddRange(activities.Select(e =>
                new ActivityApprovedIntegrationEvent(e.Id, e.ApprovedOn!.Value)));

            await context.PublishBatch(events);
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