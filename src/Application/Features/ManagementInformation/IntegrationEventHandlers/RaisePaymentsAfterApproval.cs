﻿using Cfo.Cats.Application.Features.Inductions.IntegrationEvents;
using Cfo.Cats.Application.Features.Participants.IntegrationEvents;
using MassTransit;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;

public class RaisePaymentsAfterApproval(IApplicationDbContext appDb) : IConsumer<ParticipantTransitionedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ParticipantTransitionedIntegrationEvent> context)
    {
        if (context.Message.To == EnrolmentStatus.ApprovedStatus.Name)
        {

            if (await IsFirstApproval(context.Message.ParticipantId))
            {
                var wingInductions = await appDb.WingInductions.Where(e => e.ParticipantId == context.Message.ParticipantId)
                    .ToArrayAsync();

                foreach (var wingInduction in wingInductions)
                {
                    await context.Publish(new WingInductionCreatedIntegrationEvent(wingInduction.Id));
                }

                var hubInductions = await appDb.HubInductions.Where(e => e.ParticipantId == context.Message.ParticipantId)
                    .ToArrayAsync();

                foreach (var hubInduction in hubInductions)
                {
                    await context.Publish(new HubInductionCreatedIntegrationEvent(hubInduction.Id));
                }
            }
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