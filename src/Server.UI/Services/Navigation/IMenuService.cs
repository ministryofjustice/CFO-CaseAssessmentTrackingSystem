using Cfo.Cats.Server.UI.Models.NavigationMenu;

namespace Cfo.Cats.Server.UI.Services.Navigation;

public interface IMenuService
{
    IEnumerable<MenuSectionModel> Features { get; }
}
