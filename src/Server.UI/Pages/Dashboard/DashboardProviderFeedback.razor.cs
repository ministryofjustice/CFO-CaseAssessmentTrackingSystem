using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Dashboard.Commands;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Components.Identity;

namespace Cfo.Cats.Server.UI.Pages.Dashboard;

public partial class DashboardProviderFeedback
{
    private MudDateRangePicker _picker = null!;
    private bool _showSelect;
    private bool _visualMode = true;
    private bool _downloading = false;
    private bool _showExportPanel = false;
    private bool _includeEnrolmentReturns = false;
    private bool _includeActivitiesReturns = false;
    private bool _includeEnrolmentAdvisories = false;
    private bool _includeActivitiesAdvisories = false;

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

    private void ToggleExportPanel()
    {
        _showExportPanel = !_showExportPanel;
        if (_showExportPanel)
        {
            _includeEnrolmentReturns = true;
            _includeActivitiesReturns = true;
            _includeEnrolmentAdvisories = true;
            _includeActivitiesAdvisories = true;
        }
    }

    private async Task OnExport()
    {
        try
        {
            _downloading = true;

            var startDate = _dateRange.Start ?? throw new InvalidOperationException("Start date not set");
            var endDate = _dateRange.End ?? throw new InvalidOperationException("End date not set");

            var includeEnrolmentReturns = false;
            var includeActivitiesReturns = false;
            var includeEnrolmentAdvisories = false;
            var includeActivitiesAdvisories = false;
            var skippedSheets = new List<string>();

            if (_includeEnrolmentReturns)
            {
                var result = await GetNewMediator().Send(new GetEnrolmentsToProvider.Query
                {
                    CurrentUser = CurrentUser,
                    UserId = CurrentUser.UserId,
                    TenantId = SelectedTenantId,
                    StartDate = startDate,
                    EndDate = endDate
                });
                if (result.Succeeded && result.Data?.TabularData.Length > 0)
                {
                    includeEnrolmentReturns = true;
                }
                else
                {
                    skippedSheets.Add("Enrolment: Returns");
                }
            }

            if (_includeActivitiesReturns)
            {
                var result = await GetNewMediator().Send(new GetActivitiesToProvider.Query
                {
                    CurrentUser = CurrentUser,
                    UserId = CurrentUser.UserId,
                    TenantId = SelectedTenantId,
                    StartDate = startDate,
                    EndDate = endDate
                });
                if (result.Succeeded && result.Data?.TabularData.Length > 0)
                {
                    includeActivitiesReturns = true;
                }
                else
                {
                    skippedSheets.Add("Activities: Returns");
                }
            }

            if (_includeEnrolmentAdvisories)
            {
                var result = await GetNewMediator().Send(new GetEnrolmentAdvisoriesToProvider.Query
                {
                    CurrentUser = CurrentUser,
                    UserId = CurrentUser.UserId,
                    TenantId = SelectedTenantId,
                    StartDate = startDate,
                    EndDate = endDate
                });
                if (result.Succeeded && result.Data?.TabularData.Length > 0)
                {
                    includeEnrolmentAdvisories = true;
                }
                else
                {
                    skippedSheets.Add("Enrolment: Advisories");
                }
            }

            if (_includeActivitiesAdvisories)
            {
                var result = await GetNewMediator().Send(new GetActivitiesAdvisoriesToProvider.Query
                {
                    CurrentUser = CurrentUser,
                    UserId = CurrentUser.UserId,
                    TenantId = SelectedTenantId,
                    StartDate = startDate,
                    EndDate = endDate
                });
                if (result.Succeeded && result.Data?.TabularData.Count > 0)
                {
                    includeActivitiesAdvisories = true;
                }
                else
                {
                    skippedSheets.Add("Activities: Advisories");
                }
            }

            if (!includeEnrolmentReturns && !includeActivitiesReturns && !includeEnrolmentAdvisories && !includeActivitiesAdvisories)
            {
                Snackbar.Add("No data is available to export for the selected sheets in this date range. Please adjust your date range or sheet selection and try again.", Severity.Error);
                return;
            }

            if (skippedSheets.Count > 0)
            {
                Snackbar.Add($"No data found for the following sheets — they will be skipped: {string.Join(", ", skippedSheets)}.", Severity.Info);
            }

            var exportResult = await GetNewMediator().Send(new ExportProviderFeedback.Command
            {
                Request = new ExportProviderFeedback.ProviderFeedbackExportRequest
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    TenantId = SelectedTenantId,
                    IncludeEnrolmentReturns = includeEnrolmentReturns,
                    IncludeActivitiesReturns = includeActivitiesReturns,
                    IncludeEnrolmentAdvisories = includeEnrolmentAdvisories,
                    IncludeActivitiesAdvisories = includeActivitiesAdvisories,
                }
            });

            if (exportResult.Succeeded)
            {
                Snackbar.Add($"{ConstantString.ExportSuccess}", Severity.Info);
                _showExportPanel = false;
                return;
            }

            Snackbar.Add(exportResult.ErrorMessage, Severity.Error);
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