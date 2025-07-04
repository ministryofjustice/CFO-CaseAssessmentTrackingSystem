namespace Cfo.Cats.Server.UI.Pages.Dashboard.Components;

public partial class LoadingCard
{
    [Parameter, EditorRequired] 
    public string Title { get; set; } = default!;
}