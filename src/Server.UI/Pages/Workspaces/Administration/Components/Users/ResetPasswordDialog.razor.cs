using Cfo.Cats.Application.Common.Exceptions;
using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Components.Users;

public partial class ResetPasswordDialog
{
    private bool _loading;
    private bool _isPasswordFieldDisabled = true;
    private List<KeyValuePair<char, string>> _pronunciation = [];
    private bool _expanded;
    private bool _passwordReset;
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    [EditorRequired] [Parameter] public ResetPasswordFormModel Model { get; set; } = null!;
    private MudForm? form;
    private void Close() => MudDialog.Close(DialogResult.Ok(true));

    private async Task ResetPassword()
    {
        try
        {
            _loading = true;
            if (string.IsNullOrEmpty(Model.Password))
            {
                return;
            }

            var user = await UserManager.FindByIdAsync(Model.UserId!) ??
                       throw new NotFoundException($"Application user not found {Model.UserId}.");
            var token = await UserManager.GeneratePasswordResetTokenAsync(user);
            var identityResult = await UserManager.ResetPasswordAsync(user, token, Model.Password!);

            if (identityResult.Succeeded)
            {
                if (user.EmailConfirmed is false)
                {
                    user.EmailConfirmed = true;
                }

                user.RequiresPasswordReset = true;
                await UserManager.UpdateAsync(user);
                _passwordReset = true;
            }
            else
            {
                Snackbar.Add($"{string.Join(",", identityResult.Errors.Select(x => x.Description).ToArray())}",
                    Severity.Error);
            }
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task Generate()
    {
        Model.Password = PasswordService.GeneratePassword();
        _pronunciation = PasswordService.GetPronunciation(Model.Password);

        await ResetPassword();
        _isPasswordFieldDisabled = false;
    }

    private void Cancel() => MudDialog.Cancel();
}
