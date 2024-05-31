using Cfo.Cats.Application.Common.Models;
using Cfo.Cats.Application.Features.Identity.DTOs;

namespace Cfo.Cats.Application.Common.Interfaces.Identity;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(int userId, CancellationToken cancellation = default);
    Task<bool> IsInRoleAsync(int userId, string role, CancellationToken cancellation = default);
    Task<bool> AuthorizeAsync(
        int userId,
        string policyName,
        CancellationToken cancellation = default
    );
    Task<Result> DeleteUserAsync(int userId, CancellationToken cancellation = default);
    Task<IDictionary<string, string?>> FetchUsers(
        string roleName,
        CancellationToken cancellation = default
    );
    Task UpdateLiveStatus(int userId, bool isLive, CancellationToken cancellation = default);
    Task<ApplicationUserDto> GetApplicationUserDto(
        string userName,
        CancellationToken cancellation = default
    );
    string GetUserName(int userId);
    Task<List<ApplicationUserDto>?> GetUsers(
        string? tenantId,
        CancellationToken cancellation = default
    );
}
