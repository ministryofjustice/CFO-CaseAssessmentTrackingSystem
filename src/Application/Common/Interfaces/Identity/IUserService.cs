using Cfo.Cats.Application.Features.Identity.DTOs;

namespace Cfo.Cats.Application.Common.Interfaces.Identity;

public interface IUserService
{
    List<ApplicationUserDto> DataSource { get; }
    event Action? OnChange;
    void Initialize();
    void Refresh();
    string? GetDisplayName(string userId);
}
