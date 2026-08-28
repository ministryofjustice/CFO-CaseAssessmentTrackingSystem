using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Identity.DTOs;

namespace Cfo.Cats.Server.UI.Components.Identity;

public partial class SelectUserDialog
{
    private bool _saving;

    [CascadingParameter]
    private IMudDialogInstance Dialog { get; set; } = null!;

    [Parameter]
    public UserProfile CurrentUser { get; set; } = null!;

    [Parameter]
    public bool ShowAllOption { get; set; }

    [Parameter]
    public string AllOptionLabel { get; set; } = "All Users";

    private bool _hasSelection = false;

    private SelectedUser SelectedUser { get; set; } = new(string.Empty, string.Empty);

    private void Submit()
    {
        _saving = true;
        Dialog.Close(DialogResult.Ok(SelectedUser));
    }

    private void OnUserSelectedChanged(ApplicationUserDto? dto)
    {
        SelectedUser = new SelectedUser(dto?.Id ?? string.Empty, dto?.DisplayName ?? string.Empty);
        _hasSelection = true;
    }
}

public record SelectedUser(string UserId, string DisplayName);