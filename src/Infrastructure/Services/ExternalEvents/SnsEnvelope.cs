using System.Text.Json.Serialization;

namespace Cfo.Cats.Infrastructure.Services.ExternalEvents;

/// <summary>
/// The SNS-to-SQS notification envelope. HMPPS publishes to an SNS topic which fans out to our SQS
/// queue, wrapping the actual event payload in the <see cref="Message"/> property as a JSON string.
/// </summary>
public sealed class SnsEnvelope
{
    [JsonPropertyName("Message")]
    public string? Message { get; init; }
}
