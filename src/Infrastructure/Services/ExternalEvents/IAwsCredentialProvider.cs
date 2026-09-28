using Amazon.Runtime;

namespace Cfo.Cats.Infrastructure.Services.ExternalEvents;

/// <summary>
/// Provides temporary AWS credentials obtained from the HMPPS token endpoint, hiding the credential
/// lifecycle (fetching, caching and refreshing before expiry) from the rest of the application.
/// </summary>
public interface IAwsCredentialProvider
{
    Task<SessionAWSCredentials> GetCredentialsAsync(CancellationToken cancellationToken);
}
