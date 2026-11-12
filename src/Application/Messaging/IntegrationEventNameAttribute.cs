namespace Cfo.Cats.Application.Messaging;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class IntegrationEventNameAttribute(string name, int version = 1) : Attribute
{
    public string Name { get; } = name;
    public int Version { get; } = version;
}
