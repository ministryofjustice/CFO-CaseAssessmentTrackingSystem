using Cfo.Cats.Contracts.IntegrationEvents.Enrolments;
using MassTransit;

namespace Cfo.Cats.Application.Features.Participants.Consumers;

public class EnrolmentApprovedIntegrationEventConsumer(ILogger<EnrolmentApprovedIntegrationEventConsumer> logger) : IConsumer<EnrolmentApprovedIntegrationEvent>
{
    public Task Consume(ConsumeContext<EnrolmentApprovedIntegrationEvent> context)
    {
        logger.LogInformation($"Received enrolment approval integration event for {context.Message.ParticipantId}");
        return Task.CompletedTask;
    }
}