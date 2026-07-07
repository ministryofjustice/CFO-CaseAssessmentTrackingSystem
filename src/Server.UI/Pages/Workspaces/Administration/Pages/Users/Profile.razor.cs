using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Identity.Notifications.IdentityEvents;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Pages.Users;

public partial class Profile
{
    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = null!;
    private UserManager<ApplicationUser> _userManager = null!; 
    public string Title { get; set; } = "Profile";
    private MudForm? _form;
    private MudForm? _passwordForm;
    private bool _submitting;
    private ChangePasswordModel changePassword { get; } = new();
    private UserProfile? Model { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _userManager = ScopedServices.GetRequiredService<UserManager<ApplicationUser>>();
        var state = await AuthState;

        var user = state.User;
        Model = user.GetUserProfileFromClaim();

        await base.OnInitializedAsync();
    }

    private async Task Submit()
    {
        _submitting = true;
        try
        {
            await _form!.ValidateAsync();
            if (_form.IsValid)
            {
                //var state = await AuthState;
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == Model!.UserId);
                user!.PhoneNumber = Model!.PhoneNumber;
                user.DisplayName = Model.DisplayName;
                user.ProfilePictureDataUrl = Model.ProfilePictureDataUrl;
                await _userManager.UpdateAsync(user);
                Snackbar.Add($"{ConstantString.UpdateSuccess}", Severity.Info);
            }
        }
        finally
        {
            _submitting = false;
        }
    }

    private async Task ChangePassword()
    {
        _submitting = true;
        try
        {
            await _passwordForm!.ValidateAsync();
            if (_passwordForm!.IsValid)
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == Model!.UserId);
                var result = await _userManager.ChangePasswordAsync(user!, changePassword.CurrentPassword,
                    changePassword.NewPassword);
                if (result.Succeeded)
                {
                    await GetNewMediator()
                        .Publish(IdentityAuditNotification.PasswordReset(user!.UserName!, NetworkIpProvider.IpAddress));
                    Snackbar.Add($"{L["Password changed successfully... Logging out"]}", Severity.Info);
                    Navigation.NavigateTo(@IdentityComponentsEndpointRouteBuilderExtensions.Logout, true);
                }
                else
                {
                    Snackbar.Add($"{string.Join(",", result.Errors.Select(x => x.Description).ToArray())}",
                        Severity.Error);
                }
            }
        }
        finally
        {
            _submitting = false;
        }
    }
}
