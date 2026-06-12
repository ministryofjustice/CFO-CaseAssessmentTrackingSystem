using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Identity.Commands;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Components.Shared.Layout;

public partial class UserMenu
{

    [Parameter] public EventCallback<EventArgs> OnSettingClick { get; set; }
    private bool IsLoading { get; set; }
    private UserProfile? UserProfile { get; set; }

    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        if (UserProfile is null)
        {
            IsLoading = true;
            var state = await AuthState;
            UserProfile = state.User.GetUserProfileFromClaim();
            IsLoading = false;
        }
    }

    private async Task SetHomePage()
    {
        var currentPath = Navigation.ToAbsoluteUri(Navigation.Uri).AbsolutePath;
        var homePage = string.IsNullOrWhiteSpace(currentPath) ? "/" : currentPath;

        async Task OnConfirm()
        {
            var result = await GetNewMediator().Send(new SetHomePage.Command
            {
                HomePage = homePage
            });

            if (result.Succeeded)
            {
                Snackbar.Add("Default page updated", Severity.Success);
                return;
            }

            Snackbar.Add(result.ErrorMessage, Severity.Error);
        }

        await DialogServiceHelper.ShowConfirmationDialog(
            "Confirm Home Page",
            "Set this page as your default home page?",
            OnConfirm);
    }
}