using ActualLab.Fusion;

namespace Cfo.Cats.Server.UI.Services.Fusion;

public interface IUserSessionTracker: IComputeService
{
    Task AddUserSession(string pageComponent, string userName, CancellationToken cancellationToken = default);
    Task RemoveUserSession(string pageComponent, string userName, CancellationToken cancellationToken = default);
    
    [ComputeMethod]
    Task<(string PageComponent, string[] UserSessions)[]> GetUserSessions(CancellationToken cancellationToken = default);
}