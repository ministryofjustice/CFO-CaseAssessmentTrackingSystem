using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cfo.Cats.Domain.Entities.Notifications;
using Dapper;

namespace Cfo.Cats.Server.UI.Services;

public interface INotificationService
{
    /// <summary>
    /// Event raised a refresh has taken place.
    /// </summary>
    event Action? OnRefreshed;
    
    Task<int> GetNotificationCount(string? userId, CancellationToken cancellationToken = default);
    void Refresh();

}

public class NotificationService(ISqlConnectionFactory connectionFactory) : INotificationService
{
    public event Action? OnRefreshed;

    public async Task<int> GetNotificationCount(string? userId, CancellationToken cancellationToken = default)
    {
        var sql = 
                    """
                        SELECT Count(1) FROM [Identity].[Notification]
                        WHERE [OwnerId] = @UserId AND [ReadDate] IS NULL
                          AND [NotificationDate] >= @Cutoff
                    """;

        using var connection = connectionFactory.CreateOpenConnection();

        return await connection.QuerySingleAsync<int>(
            sql,
            new
            {
                UserId = userId,
                Cutoff = NotificationConstants.ExpiryCutoff
            });
    }

    public void Refresh() => OnRefreshed?.Invoke();

}