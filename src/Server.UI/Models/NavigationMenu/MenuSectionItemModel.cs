namespace Cfo.Cats.Server.UI.Models.NavigationMenu;

public class MenuSectionItemModel
{
    public string Title { get; init; } = string.Empty;
    public string? Icon { get; init; }
    public string? Href { get; init; }
    public PageStatus PageStatus { get; init; } = PageStatus.Completed;
    public bool IsParent { get; init; }
    public List<MenuSectionSubItemModel> MenuItems { get; init; } = [];
}
