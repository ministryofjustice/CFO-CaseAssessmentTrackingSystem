using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Constants;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Account.Pages;

public partial class Profile
{
    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = null!;
    private UserManager<ApplicationUser> _userManager = null!; 
    public string Title { get; set; } = "Profile";
    private MudForm? _form;

    private bool _submitting;
    
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

    
}
