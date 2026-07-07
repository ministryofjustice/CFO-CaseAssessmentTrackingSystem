using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Identity.DTOs;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Components.Users;

public partial class UserFormDialog
{
    private bool _loading;
    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = null!;
    public UserProfile UserProfile { get; set; } = null!;
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    [Parameter] public ApplicationUserDto Model { get; set; } = null!;
    private UserForm? _userForm;

    protected async Task Submit()
    {
        try
        {
            _loading = true;
            await _userForm!.Submit();
        }
        finally
        {
            _loading = false;
        }
    }

    private void Cancel() => MudDialog.Cancel();

    protected Task OnFormSubmitHandler(ApplicationUserDto model)
    {
        MudDialog.Close(DialogResult.Ok(model));
        return Task.CompletedTask;
    }
}
