namespace Cfo.Cats.Application.Messaging;

/// <summary>
/// A fact that has happened and is of interest to other parts of the system.
/// Published with pub/sub semantics: every interested consumer receives its own copy.
/// </summary>
public interface IIntegrationEvent
{
    Guid MessageId { get; }
    DateTime OccurredOn { get; }
}
