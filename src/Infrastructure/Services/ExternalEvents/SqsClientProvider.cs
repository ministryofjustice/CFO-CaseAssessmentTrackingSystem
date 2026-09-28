using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Cfo.Cats.Infrastructure.Configurations;

namespace Cfo.Cats.Infrastructure.Services.ExternalEvents;

/// <summary>
/// Creates <see cref="IAmazonSQS"/> clients using the current temporary credentials, recreating the
/// client whenever the credentials are rotated by <see cref="IAwsCredentialProvider"/>.
/// </summary>
public class SqsClientProvider(
    IAwsCredentialProvider credentialProvider,
    IOptions<HmppsExternalApiOptions> options) : ISqsClientProvider, IDisposable
{
    private readonly HmppsExternalEventsOptions _events = options.Value.Events;
    private readonly SemaphoreSlim _gate = new(1, 1);

    private IAmazonSQS? _client;
    private SessionAWSCredentials? _currentCredentials;

    public async Task<IAmazonSQS> GetClientAsync(CancellationToken cancellationToken)
    {
        var credentials = await credentialProvider.GetCredentialsAsync(cancellationToken);

        await _gate.WaitAsync(cancellationToken);
        try
        {
            if (_client is not null && ReferenceEquals(credentials, _currentCredentials))
            {
                return _client;
            }

            _client?.Dispose();

            _client = new AmazonSQSClient(credentials, new AmazonSQSConfig
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(_events.Region)
            });
            _currentCredentials = credentials;

            return _client;
        }
        finally
        {
            _gate.Release();
        }
    }

    public void Dispose()
    {
        _client?.Dispose();
        _gate.Dispose();
        GC.SuppressFinalize(this);
    }
}
