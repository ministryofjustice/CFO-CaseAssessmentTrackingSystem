using Amazon.SQS;

namespace Cfo.Cats.Infrastructure.Services.ExternalEvents;

/// <summary>
/// Creates and refreshes <see cref="IAmazonSQS"/> clients, recreating them whenever the underlying
/// temporary credentials change. Hides the AWS SDK lifecycle concerns from the consumer.
/// </summary>
public interface ISqsClientProvider
{
    Task<IAmazonSQS> GetClientAsync(CancellationToken cancellationToken);
}
