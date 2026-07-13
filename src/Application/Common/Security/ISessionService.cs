namespace Cfo.Cats.Application.Common.Security;

public interface ISessionService
{
    Task<bool> IsSessionValidAsync(string? userId, CancellationToken cancellationToken = default);
    Task UpdateActivityAsync(string? userId, CancellationToken cancellationToken = default);
    Task InvalidateSessionAsync(string? userId, CancellationToken cancellationToken = default);

    Task StartSessionAsync(string? userId, CancellationToken cancellationToken = default);
    Task<TimeSpan?> GetRemainingSessionTimeAsync(string? userId, CancellationToken cancellationToken = default);
}
