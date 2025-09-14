using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Identity.DTOs;

namespace Cfo.Cats.Server.UI.Pages.Dashboard;

public partial class SupportWorkerPerformance
{
    private bool _showSelect;
    private bool _visualMode = true;
    public string? SelectedUserId { get; set; }
    
    [CascadingParameter]
    public UserProfile CurrentUser { get; set; } = null!;
    protected override void OnInitialized()
    {
        _showSelect = CurrentUser.AssignedRoles is { Length: > 0 };

        // if the current user has access to select, don't set the selected user.
        SelectedUserId = _showSelect ? null : CurrentUser.UserId;
    }

    private void OnUserSelectedChanged(ApplicationUserDto? dto)
    {
        SelectedUserId = dto?.Id;
    }
}
