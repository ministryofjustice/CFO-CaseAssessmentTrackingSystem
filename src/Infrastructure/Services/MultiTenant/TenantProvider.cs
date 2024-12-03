using Cfo.Cats.Application.Common.Interfaces.MultiTenant;

namespace Cfo.Cats.Infrastructure.Services.MultiTenant;

public sealed class TenantProvider : ITenantProvider
{
    private readonly IDictionary<Guid, Action> callbacks = new Dictionary<Guid, Action>();
    public string? TenantId { get; set; }
    public string? TenantName { get; set; }

    public void Unregister(Guid id)
    {
        if (callbacks.ContainsKey(id))
        {
            callbacks.Remove(id);
        }
    }

    public void Clear()
    {
        callbacks.Clear();
    }

    public void Update()
    {
        foreach (var callback in callbacks.Values)
        {
            callback();
        }
    }

    public Guid Register(Action callback)
    {
        var id = Guid.CreateVersion7();
        callbacks.Add(id, callback);
        return id;
    }
}
