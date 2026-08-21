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

    private SelectedUser SelectedUser { get; set; } = new SelectedUser(string.Empty, string.Empty);

    private void Submit()
    {
        _saving = true;
        Dialog.Close(DialogResult.Ok(SelectedUser));
    }

    private void OnUserSelectedChanged(ApplicationUserDto? dto) => SelectedUser = new SelectedUser(
        dto?.Id ?? string.Empty,
        dto?.DisplayName ?? string.Empty
    );
}

public record SelectedUser(string UserId, string DisplayName);