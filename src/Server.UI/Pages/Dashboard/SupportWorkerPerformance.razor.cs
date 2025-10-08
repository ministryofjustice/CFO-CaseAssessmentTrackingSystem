using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Server.UI.Components.Identity;
using CsvHelper.Configuration.Attributes;

namespace Cfo.Cats.Server.UI.Pages.Dashboard;

public partial class SupportWorkerPerformance
{

    private MudDateRangePicker _picker = null!; 

    private bool _showSelect;
    private bool _visualMode = true;
    public string? SelectedUserId { get; set; }
    public string? SelectedDisplayName { get; set; }

    private DateRange _dateRange { get; set; } = new DateRange(new DateTime(2025, 1, 1), DateTime.Today);
    
    [CascadingParameter]
    public UserProfile CurrentUser { get; set; } = null!;
    protected override void OnInitialized()
    {
        _showSelect = CurrentUser.AssignedRoles is { Length: > 0 };

        // if the current user has access to select, don't set the selected user.
        SelectedUserId = CurrentUser.UserId;
        SelectedDisplayName = CurrentUser.DisplayName;
    }

    private async Task DisplayOptionsDialog()
	{
        var parameters = new DialogParameters<SelectUserDialog>
        {
            { "CurrentUser", CurrentUser }           
        };

        var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = false};
        var dialog = await DialogService.ShowAsync<SelectUserDialog>("Dashboard Options", parameters, options);
        var result = await dialog.Result;

        if(result is { Canceled: false, Data: SelectedUser user})
        {
            SelectedUserId = user.UserId;
            SelectedDisplayName = user.DisplayName;
        }
    }
}
