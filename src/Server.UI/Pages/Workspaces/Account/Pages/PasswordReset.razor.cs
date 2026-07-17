using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Identity.Notifications.IdentityEvents;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Services;
using Cfo.Cats.Server.UI.Services;
using DocumentFormat.OpenXml.Drawing;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Account.Pages;

public partial class PasswordReset
{
    private MudForm? _passwordForm;
    private bool _submitting = false;

    private ChangePasswordModel changePassword { get; } = new();

    private UserManager<ApplicationUser> _userManager = null!;

    [CascadingParameter]
    public UserProfile CurrentUser { get; set; } = default!;

    [Inject]
    public INetworkIpProvider NetworkIpProvider { get; set; } = default!;

    protected override void OnInitialized() => _userManager = ScopedServices.GetRequiredService<UserManager<ApplicationUser>>();

    private async Task ChangePassword()
    {
        _submitting = true;
        try
        {
            await _passwordForm!.ValidateAsync();
            if (_passwordForm!.IsValid)
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == CurrentUser.UserId);
                var result = await _userManager.ChangePasswordAsync(user!, changePassword.CurrentPassword,
                    changePassword.NewPassword);
                if (result.Succeeded)
                {
                    await Service.Publish(IdentityAuditNotification.PasswordReset(user!.UserName!, NetworkIpProvider.IpAddress));
                    Snackbar.Add("Password changed successfully... Logging out", Severity.Info);
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