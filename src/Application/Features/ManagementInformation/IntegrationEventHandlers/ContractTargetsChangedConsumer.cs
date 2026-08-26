using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cfo.Cats.Application.Features.ManagementInformation.IntegrationEvents;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;

public class ContractTargetsChangedConsumer(ITargetsProvider provider, ILogger<ContractTargetsChangedConsumer> logger) : IHandleMessages<TargetsChangedIntegrationEvent>
{
    public Task Handle(TargetsChangedIntegrationEvent message)
    {
        logger.LogDebug("Contract targets changed. Refreshing the cache");
        provider.Refresh();
        return Task.CompletedTask;
    }
}