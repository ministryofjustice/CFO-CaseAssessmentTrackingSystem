namespace Cfo.Cats.Infrastructure.Messaging.RabbitMq;

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitSettings";
    public const string ConnectionStringName = "rabbit";

    /// <summary>Durable topic exchange all integration events are published to.</summary>
    public string Exchange { get; set; } = "integration-events";

    /// <summary>
    /// Maximum number of times a message is delivered to a consumer before it is dead-lettered.
    /// A value of 1 means no retries.
    /// </summary>
    public int Retries { get; set; } = 2;

    /// <summary>Delay before the first retry. Each subsequent retry doubles it.</summary>
    public int RetryBaseDelaySeconds { get; set; } = 5;
}
