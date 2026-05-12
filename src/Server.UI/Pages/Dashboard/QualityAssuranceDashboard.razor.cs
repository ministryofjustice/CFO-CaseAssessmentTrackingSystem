using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Dashboard.Commands;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Components.Identity;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Dashboard;

public partial class QualityAssuranceDashboard
{
    private MudDateRangePicker _picker = null!;
    
    private bool _visualMode = true;
    private bool _downloading;

    private string SelectedTenantId { get; set; } = string.Empty;
    private string SelectedDisplayName { get; set; } = string.Empty;
    
    private DateRange DateRange { get; set; } = new DateRange(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), DateTime.Today);
    
    [CascadingParameter]
    public UserProfile CurrentUser { get; set; } = null!;
    
    [CascadingParameter] 
    private Task<AuthenticationState> AuthState { get; set; } = null!;
    
    protected override Task OnInitializedAsync()
    {
        try
        {
            SelectedTenantId = CurrentUser.TenantId
                               ?? throw new InvalidOperationException("Current user TenantId is required.");

            SelectedDisplayName = CurrentUser.TenantName
                                  ?? throw new InvalidOperationException("Current user TenantName is required.");

            return Task.CompletedTask;
        }
        catch (Exception exception)
        {
            return Task.FromException(exception);
        }
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

            var startDate = DateRange.Start ?? throw new InvalidOperationException("Start date not set");
            var endDate = DateRange.End ?? throw new InvalidOperationException("End date not set");

            var exportResult = await GetNewMediator().Send(new ExportProviderFeedback.Command
            {
                Request = new ExportProviderFeedback.ProviderFeedbackExportRequest
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    TenantId = SelectedTenantId
                }
            });

            if (exportResult.Succeeded)
            {
                Snackbar.Add($"{ConstantString.ExportSuccess}", Severity.Info);
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