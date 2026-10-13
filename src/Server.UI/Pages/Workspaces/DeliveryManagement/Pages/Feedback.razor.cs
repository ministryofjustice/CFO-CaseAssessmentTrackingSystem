using Cfo.Cats.Application.Features.Dashboard.Commands;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Infrastructure.Constants;
using MudBlazor;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Pages;

public partial class Feedback
{
    private MudDateRangePicker _picker = null!;
    private bool _visualMode = true;
    private bool _downloading;
    private DateRange _dateRange = new(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), DateTime.Today);

    protected override Task OnInitializedAsync() => Task.CompletedTask;

    protected override IQuery<Result<GetEnrolmentsToProvider.EnrolmentToProviderDto>> CreateQuery()
        => new GetEnrolmentsToProvider.Query
        {
            StartDate = _dateRange.Start ?? throw new InvalidOperationException("Start date not set"),
            EndDate = _dateRange.End ?? throw new InvalidOperationException("End date not set"),
            TenantId = TenantId,
            IncludeInternalData = false,
            CurrentUser = CurrentUser
        };

    private string TenantId => CurrentUser.TenantId
        ?? throw new InvalidOperationException("Current user TenantId is required.");

    private string FeedbackKey => TenantId + (_dateRange.Start?.Ticks ?? 0) + (_dateRange.End?.Ticks ?? 0);

    private async Task OnExport()
    {
        try
        {
            _downloading = true;

            var result = await Service.Send(new ExportDeliveryFeedback.Command
            {
                StartDate = _dateRange.Start ?? throw new InvalidOperationException("Start date not set"),
                EndDate = _dateRange.End ?? throw new InvalidOperationException("End date not set")
            });

            Snackbar.Add(result.Succeeded ? ConstantString.ExportSuccess : result.ErrorMessage,
                result.Succeeded ? Severity.Info : Severity.Error);
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "Error requesting delivery feedback export.");
            Snackbar.Add("An error occurred while generating your document.", Severity.Error);
        }
        finally
        {
            _downloading = false;
        }
    }
}