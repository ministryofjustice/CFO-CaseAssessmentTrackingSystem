using Cfo.Cats.Application.Features.Identity.DTOs;

namespace Cfo.Cats.Application.Common.Interfaces.Identity;

public interface IUserService
{
    IReadOnlyList<ApplicationUserDto> DataSource { get; }
    event Action? OnChange;
    void Refresh();
    string? GetDisplayName(string userId);
}
