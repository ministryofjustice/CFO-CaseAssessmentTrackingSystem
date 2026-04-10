namespace Cfo.Cats.Server.UI.Models.NavigationMenu;

public class MenuSectionModel
{
    public string Title { get; set; } = string.Empty;
    public List<MenuSectionItemModel> SectionItems { get; init; } = [];
}
