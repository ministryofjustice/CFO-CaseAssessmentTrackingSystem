using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cfo.Cats.Application.Features.ManagementInformation.IntegrationEvents;
using Cfo.Cats.Application.Messaging;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;

public class ContractTargetsChangedConsumer(ITargetsProvider provider, ILogger<ContractTargetsChangedConsumer> logger) : IIntegrationEventHandler<TargetsChangedIntegrationEvent>
{
    public Task Handle(TargetsChangedIntegrationEvent message, CancellationToken cancellationToken)
    {
        logger.LogDebug("Contract targets changed. Refreshing the cache");
        provider.Refresh();
        return Task.CompletedTask;
    }
}