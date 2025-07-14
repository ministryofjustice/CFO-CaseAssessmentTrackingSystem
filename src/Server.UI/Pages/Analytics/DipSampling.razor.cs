using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Contracts.DTOs;
using Cfo.Cats.Application.Features.ManagementInformation.DTOs;
using Cfo.Cats.Application.Features.ManagementInformation.Queries;

namespace Cfo.Cats.Server.UI.Pages.Analytics;

public partial class DipSampling
{
    bool isLoading;

    [CascadingParameter] public UserProfile? CurrentUser { get; set; }

    public DipSampleDto[] samples = [];

    public ContractDto? SelectedContract { get; set; }

    public GetDipSamples.Query Query { get; set; } = new();

    protected override async Task OnInitializedAsync() => await ReloadAsync();

    async Task ReloadAsync()
    {
        try
        {
            isLoading = true;

            var result = await GetNewMediator().Send(Query);

            if (result is { Succeeded: true, Data: not null })
            {
                samples = result.Data.ToArray();
                return;
            }

            Snackbar.Add(result.ErrorMessage, Severity.Error);

        }
        finally { isLoading = false; }
    }

}
