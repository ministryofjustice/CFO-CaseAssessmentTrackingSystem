using Cfo.Cats.Application.Features.PerformanceManagement.IntegrationEvents;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.PerformanceManagement.IntegrationEventHandlers;

public class GenerateDipSampleDocument(ILogger<GenerateDipSampleDocument> logger) : IHandleMessages<OutcomeQualityDipSampleFinalisingIntegrationEvent>
{
    public async Task Handle(OutcomeQualityDipSampleFinalisingIntegrationEvent message)
    {
        logger.LogInformation("Finalised event raised. Now we need to generate the document and set the status to finalised once that is done");
        await Task.CompletedTask;
    }
}
