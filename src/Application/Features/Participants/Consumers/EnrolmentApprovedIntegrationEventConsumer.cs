using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cfo.Cats.Application.Features.Participants.Contracts;
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