using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Cfo.Cats.Application.Messaging;

namespace Cfo.Cats.Infrastructure.Messaging.RabbitMq;

/// <summary>
/// Resolves event names from <see cref="IntegrationEventNameAttribute"/>. Events without the attribute
/// get a name derived from their feature folder and type name, e.g.
/// <c>Features.Participants.IntegrationEvents.ParticipantCreatedIntegrationEvent</c> becomes
/// <c>participants.participant-created</c>.
/// </summary>
public sealed partial class EventNameResolver : IEventNameResolver
{
    private readonly Dictionary<Type, (string Name, int Version)> _byType = [];
    private readonly Dictionary<(string Name, int Version), Type> _byName = [];

    public EventNameResolver(IEnumerable<Assembly> assemblies)
    {
        var eventTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t is { IsAbstract: false, IsInterface: false } && typeof(IIntegrationEvent).IsAssignableFrom(t));

        foreach (var type in eventTypes)
        {
            var attribute = type.GetCustomAttribute<IntegrationEventNameAttribute>(inherit: false);
            var name = attribute?.Name ?? Derive(type);
            var version = attribute?.Version ?? 1;

            if (!_byName.TryAdd((name, version), type))
            {
                throw new InvalidOperationException(
                    $"Integration events {_byName[(name, version)].FullName} and {type.FullName} resolve to the same name '{name}' (version {version}).");
            }

            _byType[type] = (name, version);
        }
    }

    public string GetName(Type eventType) => Get(eventType).Name;

    public int GetVersion(Type eventType) => Get(eventType).Version;

    public Type ResolveType(string eventName, int version) =>
        _byName.TryGetValue((eventName, version), out var type)
            ? type
            : throw new KeyNotFoundException($"No integration event is registered with name '{eventName}' and version {version}.");

    public IReadOnlyCollection<Type> GetAllEventTypes() => _byType.Keys;

    private (string Name, int Version) Get(Type eventType) =>
        _byType.TryGetValue(eventType, out var value)
            ? value
            : throw new KeyNotFoundException($"{eventType.FullName} is not a registered integration event.");

    private static string Derive(Type type)
    {
        var typeName = type.Name;
        const string suffix = "IntegrationEvent";
        if (typeName.EndsWith(suffix, StringComparison.Ordinal) && typeName.Length > suffix.Length)
        {
            typeName = typeName[..^suffix.Length];
        }

        var segments = (type.Namespace ?? string.Empty).Split('.');
        var featureIndex = Array.IndexOf(segments, "Features");
        var feature = featureIndex >= 0 && featureIndex + 1 < segments.Length ? Kebab(segments[featureIndex + 1]) : null;

        return feature is null ? Kebab(typeName) : $"{feature}.{Kebab(typeName)}";
    }

    private static string Kebab(string value)
    {
        var builder = new StringBuilder(WordBoundary().Replace(value, "-$1"));
        return builder.ToString().Trim('-').ToLowerInvariant();
    }

    [GeneratedRegex("(?<=[a-z0-9])([A-Z])|(?<=[A-Z])([A-Z][a-z])")]
    private static partial Regex WordBoundary();
}
