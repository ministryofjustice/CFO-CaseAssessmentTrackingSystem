namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Pages.Users;

public partial class SelectRoleDialog
{
    [CascadingParameter]
    private IMudDialogInstance Dialog { get; set; } = null!;

    [Parameter, EditorRequired]
    public IEnumerable<string> Roles { get; set; } = null!;

    private string _searchTerm = string.Empty;

    private IEnumerable<string> FilteredRoles => 
        string.IsNullOrWhiteSpace(_searchTerm)
            ? Roles
            : Roles.Where(r => r.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase));

    private void SelectRole(string role) => Dialog.Close(DialogResult.Ok(role));

    private void Cancel() => Dialog.Cancel();
}
