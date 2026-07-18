using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Features.Dashboard.Export;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Components;

public partial class MyRiskDueComponent : CatsComponent<RiskDueDto[]>
{
    private bool _approvedOnly = true;
    private string _searchString = "";
    private bool _downloading = false;

    private GetRiskDueDashboard.Query Query { get; set; } = default!;

    protected override IQuery<Result<RiskDueDto[]>> CreateQuery()
    {
        Query = new GetRiskDueDashboard.Query()
        {
            UserId = CurrentUser.UserId,
            ApprovedOnly = _approvedOnly,
            FuturesDays = 14
        };
        return Query;
    }

    private async Task OnApprovedOnlyChanged(bool value)
    {
        _approvedOnly = value;
        await RefreshAsync();
    }

    private bool FilterFunc(RiskDueDto data) => FilterFunc(data, _searchString);

    private bool FilterFunc(RiskDueDto data, string searchString)
    {
        if (string.IsNullOrEmpty(searchString))
        {
            return true;
        }

        if (data.ParticipantId.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }

        if (data.FirstName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }

        if (data.FirstName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }

        if ($"{data.FirstName} {data.LastName}".Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }

        return false;
    }

    private async Task OnExport()
    {
        try
        {
            _downloading = true;
            var result = await Service.Send(new ExportRiskDue.Command()
            {
                Query = Query
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
            Snackbar.Add($"An error occurred while generating your document.", Severity.Error);
        }
        finally
        {
            _downloading = false;
        }
    }
}