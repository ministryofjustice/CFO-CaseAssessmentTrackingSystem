using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Cfo.Cats.Infrastructure.Messaging.RabbitMq;

/// <summary>
/// Owns the single shared broker connection. Connecting is asynchronous and lazy, so it can never block
/// a Blazor render context; <see cref="RabbitMqWarmupService"/> connects at host startup.
/// </summary>
public sealed class RabbitMqConnection(IConfiguration configuration, ILogger<RabbitMqConnection> logger) : IAsyncDisposable
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private IConnection? _connection;

    public async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
    {
        if (_connection is { IsOpen: true })
        {
            return _connection;
        }

        await _gate.WaitAsync(cancellationToken);
        try
        {
            if (_connection is { IsOpen: true })
            {
                return _connection;
            }

            var connectionString = configuration.GetConnectionString(RabbitMqOptions.ConnectionStringName)
                ?? throw new InvalidOperationException($"Connection string '{RabbitMqOptions.ConnectionStringName}' is not configured.");

            var factory = new ConnectionFactory
            {
                Uri = new Uri(connectionString),
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true,
                ClientProvidedName = AppDomain.CurrentDomain.FriendlyName
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            logger.LogInformation("Connected to RabbitMQ at {Endpoint}", _connection.Endpoint);
            return _connection;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }

        _gate.Dispose();
    }
}
