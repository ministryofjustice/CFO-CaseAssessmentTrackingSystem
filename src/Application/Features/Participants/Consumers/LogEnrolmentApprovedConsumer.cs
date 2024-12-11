using Cfo.Cats.Contracts.IntegrationEvents.Enrolments;
using MassTransit;

namespace Cfo.Cats.Application.Features.Participants.Consumers
{
    public class LogEnrolmentApprovedConsumer(ILogger<LogEnrolmentApprovedConsumer> logger) : IConsumer<EnrolmentApprovedIntegrationEvent>
    {
        public Task Consume(ConsumeContext<EnrolmentApprovedIntegrationEvent> context)
        {
            logger.LogInformation($"Enrolment approved for {context.Message.ParticipantId}");
            return Task.CompletedTask;
        }
    }


    public class ErroringEnrolmentApprovedConsumer(ILogger<LogEnrolmentApprovedConsumer> logger) : IConsumer<EnrolmentApprovedIntegrationEvent>
    {
        public Task Consume(ConsumeContext<EnrolmentApprovedIntegrationEvent> context)
        {
            logger.LogInformation($"Another handler logging the Enrolment approved for {context.Message.ParticipantId}");
            throw new Exception("This simulates an exception occurring in a event handler");
        }
    }
}
