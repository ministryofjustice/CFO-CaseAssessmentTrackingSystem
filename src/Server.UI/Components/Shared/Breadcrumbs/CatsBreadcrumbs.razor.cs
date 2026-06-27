namespace Cfo.Cats.Server.UI.Components.Shared.Breadcrumbs;

public partial class CatsBreadcrumbs
{
    [Parameter, EditorRequired] public IReadOnlyList<BreadcrumbItem> Items { get; set; } = null!;
}