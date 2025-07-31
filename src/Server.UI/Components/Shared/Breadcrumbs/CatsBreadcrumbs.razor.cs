namespace Cfo.Cats.Server.UI.Components.Shared.Breadcrumbs;

public partial class CatsBreadcrumbs
{
    [Parameter] public Typo Typo { get; set; } = MudBlazor.Typo.h5;
    [Parameter, EditorRequired] public IReadOnlyList<BreadcrumbItem> Items { get; set; } = null!;
}