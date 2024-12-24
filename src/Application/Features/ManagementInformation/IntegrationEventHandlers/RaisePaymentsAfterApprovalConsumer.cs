using Cfo.Cats.Application.Features.Activities.IntegrationEvents;
using Cfo.Cats.Application.Features.Inductions.IntegrationEvents;
using Cfo.Cats.Application.Features.Participants.IntegrationEvents;
using MassTransit;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;

public class RaisePaymentsAfterApprovalConsumer(IApplicationDbContext appDb) : IConsumer<ParticipantTransitionedIntegrationEvent>
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

            var wings = await appDb.WingInductions
                .Where(e => e.ParticipantId == context.Message.ParticipantId)
                .Select(e => e.Id)
                .ToArrayAsync();

            var hubs = await appDb.HubInductions
                .Where(e => e.ParticipantId == context.Message.ParticipantId)
                .Select(e => e.Id)
                .ToArrayAsync();

            var activities = await appDb.Activities.Where(e => e.ParticipantId == context.Message.ParticipantId)
                .Where(e => e.ApprovedOn != null)
                .Select(e => new { e.Id, e.ApprovedOn })
                .ToArrayAsync();

            events.AddRange(wings.Select(wi => new WingInductionCreatedIntegrationEvent(wi)));
            events.AddRange(hubs.Select(hi => new HubInductionCreatedIntegrationEvent(hi)));
            events.AddRange(activities.Select(e =>
                new ActivityApprovedIntegrationEvent(e.Id, e.ApprovedOn!.Value)));

            await context.PublishBatch(events);
        }
    }

    private async Task<bool> IsFirstApproval(string participantId)
    {
        var history = await appDb.ParticipantEnrolmentHistories
            .Where(e => e.ParticipantId == participantId)
            .ToArrayAsync();

        return history.Count(e => e.EnrolmentStatus == EnrolmentStatus.ApprovedStatus) <= 1;
    }
}