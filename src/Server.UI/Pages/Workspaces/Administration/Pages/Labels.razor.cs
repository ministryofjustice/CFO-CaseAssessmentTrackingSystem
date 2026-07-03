namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Pages;

public partial class Labels
{
    private string Title { get; set; } = "";

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Title = L["Labels"];
    }
}
