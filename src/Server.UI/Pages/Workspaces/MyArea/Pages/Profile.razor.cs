using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Constants;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.MyArea.Pages;

public partial class Profile
{
    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = null!;
    
    public string Title { get; set; } = "Profile";
    private MudForm? _form;

    private bool _submitting;
    
    private UserProfile? Model { get; set; }

    protected override async Task OnInitializedAsync()
    {
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
                var user = await Service.Users.FirstOrDefaultAsync(u => u.Id == Model!.UserId);
                user!.PhoneNumber = Model!.PhoneNumber;
                user.DisplayName = Model.DisplayName;
                user.ProfilePictureDataUrl = Model.ProfilePictureDataUrl;
                await Service.UpdateAsync(user);
                Snackbar.Add($"{ConstantString.UpdateSuccess}", Severity.Info);
            }
        }
        finally
        {
            _submitting = false;
        }
    }

    
}
