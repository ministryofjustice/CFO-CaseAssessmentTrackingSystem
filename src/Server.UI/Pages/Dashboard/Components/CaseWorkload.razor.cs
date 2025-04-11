using BlazorDownloadFile;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Features.Dashboard.Export;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Application.Features.KeyValues.Queries.Export;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Pages.Dashboard.Components;

public partial class CaseWorkload
{

    private bool _downloading = false;
    private string _searchString = "";


    [CascadingParameter] 
    public UserProfile UserProfile { get; set; } = default!;

    [Inject] public IBlazorDownloadFileService BlazorDownloadFileService { get; set; } = default!;

    private GetCaseWorkload.Query Query { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await OnRefresh();
    }

    private async Task OnRefresh()
    {
        Query.CurrentUser = UserProfile;
        Result = await GetNewMediator().Send(Query).ConfigureAwait(false);
    }

    private Result<CaseSummaryDto[]>? Result { get; set; }

    private async Task OnExport()
    {
        _downloading = true;
        var request = new ExportCaseWorkload.Query()
        {
           CurrentUser = UserProfile
        };
        var result = await GetNewMediator().Send(request);
        var downloadResult = await BlazorDownloadFileService.DownloadFile($"CaseWorkLoad.xlsx", result, "application/octet-stream");
        Snackbar.Add($"{ConstantString.ExportSuccess}", Severity.Info);
        _downloading = false;
    }

    private bool FilterFunc(CaseSummaryDto data) => FilterFunc(data, _searchString);

    private bool FilterFunc(CaseSummaryDto data, string searchString)
    {
        if (string.IsNullOrEmpty(searchString))
        {
            return true;
        }

        if (data.UserName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }

        if (data.LocationName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }

        if (data.TenantName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }

        if (data.GetEnrolmentStatus().Name.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }

        return false;
    }


}