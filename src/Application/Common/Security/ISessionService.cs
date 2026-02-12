namespace Cfo.Cats.Application.Common.Security;

public interface ISessionService
{
    bool IsSessionValid(string? userId);
    void UpdateActivity(string? userId);
    void InvalidateSession(string? userId);
    
    void StartSession(string? userId);
    TimeSpan? GetRemainingSessionTime(string? userId);
}
