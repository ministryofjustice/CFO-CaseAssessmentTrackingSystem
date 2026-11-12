using Cfo.Cats.Application.Common.Configuration;
using Cfo.Cats.Application.Features.Participants.IntegrationEvents;
using Microsoft.Extensions.Configuration;
using Cfo.Cats.Application.Messaging;

namespace Cfo.Cats.Application.Features.Participants.IntegrationEventHandlers;

/// <summary>
/// Consumes <see cref="ParticipantCreatedIntegrationEvent"/> (always published via the outbox when a participant
/// is created) and, when the "Features:UseDmsHardLinkApi" flag is enabled, calls the DMS hard link API directly
/// via <see cref="ICandidateService"/>. When the flag is disabled, this is a no-op - DMS is instead expected to
/// be notified via its own subscription to the published message.
/// </summary>
public class ParticipantCreatedIntegrationEventConsumer(
    ICandidateService candidateService,
    IConfiguration configuration,
    ILogger<ParticipantCreatedIntegrationEventConsumer> logger) : IIntegrationEventHandler<ParticipantCreatedIntegrationEvent>
{
    public async Task Handle(ParticipantCreatedIntegrationEvent message, CancellationToken cancellationToken)
    {
        try
        {
            if(configuration.IsFeatureEnabled(FeatureSections.UseDmsHardLinkApi))
            {
                await candidateService.SetHardLink(message.ParticipantId, message.PrimaryRecordKeyAtCreation, message.OccurredOn);                
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to call DMS hard link API for participant {ParticipantId}", message.ParticipantId);
        }
    }
}
