using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Contracts.DTOs;
using Cfo.Cats.Application.Features.ManagementInformation.DTOs;
using Cfo.Cats.Application.Features.ManagementInformation.Queries;
using Cfo.Cats.Infrastructure.Configurations;
using Microsoft.Extensions.Options;

namespace Cfo.Cats.Server.UI.Pages.Analytics;

public partial class OutcomeQualityDipSampling
{
    bool isLoading;

    [CascadingParameter] public UserProfile? CurrentUser { get; set; }

    [Inject] public required IOptions<OutcomeQualityDipSampleSettings> Options { get; set; }

    public DipSampleDto[] samples = [];

    public ContractDto? SelectedContract { get; set; }

    public GetOutcomeQualityDipSamples.Query Query { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        var offset = DateTime.Now.AddMonths(Options.Value.MonthOffset);

        Query = new()
        {
            Month = offset.Month,
            Year = offset.Year
        };

        await ReloadAsync();
    }

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
