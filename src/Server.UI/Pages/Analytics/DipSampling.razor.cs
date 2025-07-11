using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Contracts.DTOs;
using Cfo.Cats.Application.Features.ManagementInformation.DTOs;
using Cfo.Cats.Application.Features.ManagementInformation.Queries;

namespace Cfo.Cats.Server.UI.Pages.Analytics;

public partial class DipSampling
{
    bool isLoading;

    [CascadingParameter] public UserProfile? CurrentUser { get; set; }

    public int Month { get; set; } = DateTime.Now.AddMonths(-4).Month;
    public int Year { get; set; } = DateTime.Now.AddMonths(-4).Year;

    public DipSampleDto[] samples = [];

    public ContractDto? SelectedContract { get; set; }

    public GetDipSamples.Query Query { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            isLoading = true;

            Query = new() { Month = Month, Year = Year };

            var result = await GetNewMediator().Send(Query);

            if(result is { Succeeded: true, Data: not null })
            {
                samples = result.Data.ToArray();
                return;
            }

            Snackbar.Add(result.ErrorMessage, Severity.Error);

        }
        finally { isLoading = false; }
    }

}
