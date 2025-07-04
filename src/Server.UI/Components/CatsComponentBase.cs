﻿namespace Cfo.Cats.Server.UI.Components;

public abstract class CatsComponentBase : OwningComponentBase
{

    protected CancellationToken ComponentCancellationToken => _cts.Token;
    private readonly CancellationTokenSource _cts = new();
    
    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = default!;

    private readonly List<IServiceScope> _scopes = new();
    private bool _disposed = false;

    [Parameter]
    public EventCallback OnUpdate { get; set; } = new EventCallback();
    
    protected IMediator GetNewMediator()
    {
        // Create a new scope and return a new instance of IMediator
        var scope = ServiceProvider.CreateScope();
        _scopes.Add(scope);
        return scope.ServiceProvider.GetRequiredService<IMediator>();
    }

    protected IUnitOfWork GetNewUnitOfWork()
    {
        // Create a new scope and return a new instance of IMediator
        var scope = ServiceProvider.CreateScope();
        _scopes.Add(scope);
        return scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _cts.Cancel();
                _cts.Dispose();
                
                // Dispose all scopes created for Mediatr instances
                foreach (var scope in _scopes)
                {
                    scope.Dispose();
                }
                _scopes.Clear();
            }
            _disposed = true;
        }
        base.Dispose(disposing);
    }
    
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    
}
