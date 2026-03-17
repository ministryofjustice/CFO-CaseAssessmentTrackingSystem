using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Dashboard.Commands;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Components.Identity;

namespace Cfo.Cats.Server.UI.Pages.Dashboard;

public partial class DashboardProviderFeedback
{
    private MudDateRangePicker _picker = null!;
    private bool _showSelect;
    private bool _visualMode = true;
    private bool _downloading = false;
    private bool _includeEnrolmentReturns = true;
    private bool _includeActivitiesReturns = true;
    private bool _includeEnrolmentAdvisories = true;
    private bool _includeActivitiesAdvisories = true;

    public string? SelectedTenantId { get; set; }
    public string? SelectedUserId { get; set; }
    public string? SelectedDisplayName { get; set; }
    private DateRange _dateRange { get; set; } = new DateRange(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), DateTime.Today);
    
    [CascadingParameter]
    public UserProfile CurrentUser { get; set; } = null!;

    protected override void OnInitialized()
    {
        _showSelect = CurrentUser.AssignedRoles is { Length: > 0 };
        SelectedTenantId = CurrentUser.TenantId;
        SelectedDisplayName = CurrentUser.TenantName;
    }

    private async Task DisplayOptionsDialog()
	{
        var parameters = new DialogParameters<SelectTenantDialog>
        {
            { "CurrentUser", CurrentUser }           
        };

        var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = false};
        var dialog = await DialogService.ShowAsync<SelectTenantDialog>("Dashboard Options", parameters, options);
        var result = await dialog.Result;

        if(result is { Canceled: false, Data: SelectedTenant tenant})
        {
            SelectedTenantId = tenant.TenantId;
            SelectedDisplayName = tenant.DisplayName;
        }
    }

    private async Task OnExport()
    {
        try
        {
            _downloading = true;
            var result = await GetNewMediator().Send(new ExportProviderFeedback.Command
            {
                Request = new ExportProviderFeedback.ProviderFeedbackExportRequest
                {
                    StartDate = _dateRange.Start ?? throw new InvalidOperationException("Start date not set"),
                    EndDate = _dateRange.End ?? throw new InvalidOperationException("End date not set"),
                    TenantId = SelectedTenantId,
                    IncludeEnrolmentReturns = _includeEnrolmentReturns,
                    IncludeActivitiesReturns = _includeActivitiesReturns,
                    IncludeEnrolmentAdvisories = _includeEnrolmentAdvisories,
                    IncludeActivitiesAdvisories = _includeActivitiesAdvisories,
                }
            });

            if (result.Succeeded)
            {
                Snackbar.Add($"{ConstantString.ExportSuccess}", Severity.Info);
                return;
            }

            Snackbar.Add(result.ErrorMessage, Severity.Error);
        }
        catch
        {
            Snackbar.Add("An error occurred while generating your document.", Severity.Error);
        }
        finally
        {
            _downloading = false;
        }
    }
}