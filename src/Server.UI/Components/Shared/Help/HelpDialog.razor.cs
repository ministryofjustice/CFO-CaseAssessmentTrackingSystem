using Cfo.Cats.Application.Features.HelpLinks.DTOs;

namespace Cfo.Cats.Server.UI.Components.Shared.Help;

/// <summary>
/// Shows the help content (if any) configured for the current page/tab. System Support users
/// always see diagnostic details (page key, tab name, whether a link is set) plus a shortcut to
/// the Help Links admin page, pre-filtered to the current page/tab, so gaps can be filled in
/// quickly while browsing the app.
/// </summary>
public partial class HelpDialog
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;

    [Parameter]
    public HelpLinkDto? HelpLink { get; set; }

    [Parameter, EditorRequired]
    public string PageKey { get; set; } = string.Empty;

    [Parameter]
    public string? TabName { get; set; }

    [Parameter]
    public bool IsSystemSupport { get; set; }

    private void Close() => MudDialog.Close();

    private void ManageHelpLinks()
    {
        var uri = $"/pages/workspace/administration/helplinks?pageKey={Uri.EscapeDataString(PageKey)}";

        if (TabName is not null)
        {
            uri += $"&tabName={Uri.EscapeDataString(TabName)}";
        }

        MudDialog.Close();
        Navigation.NavigateTo(uri);
    }
}
