namespace Cfo.Cats.Application.Messaging;

/// <summary>
/// Maps integration event CLR types to stable, transport-neutral names (e.g. "participant.created")
/// so that renaming or moving a type never changes what is on the wire.
/// </summary>
public interface IEventNameResolver
{
    string GetName(Type eventType);
    Type ResolveType(string eventName, int version);
    int GetVersion(Type eventType);
    IReadOnlyCollection<Type> GetAllEventTypes();
}
