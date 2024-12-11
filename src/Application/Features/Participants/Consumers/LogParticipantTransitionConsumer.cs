using Cfo.Cats.Application.Features.Participants.IntegrationEvents;
using MassTransit;

namespace Cfo.Cats.Application.Features.Participants.Consumers;

public class LogParticipantTransitionConsumer(ILogger<LogParticipantTransitionConsumer> logger) : IConsumer<ParticipantTransitionedIntegrationEvent>
{
    public Task Consume(ConsumeContext<ParticipantTransitionedIntegrationEvent> context)
    {
        var (participantId, from, to) = context.Message;
        logger.LogInformation($"Participant {participantId} transitioned from {from} to {to}");
        return Task.CompletedTask;
    }
}