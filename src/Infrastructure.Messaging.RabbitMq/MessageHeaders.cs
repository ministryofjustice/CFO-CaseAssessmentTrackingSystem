namespace Cfo.Cats.Infrastructure.Messaging.RabbitMq;

internal static class MessageHeaders
{
    public const string Version = "cats-version";
    public const string OccurredOnUtc = "cats-occurred-on-utc";
    public const string Attempt = "cats-attempt";
    public const string Error = "cats-error";
}
