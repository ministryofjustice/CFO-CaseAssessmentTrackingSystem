using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Contracts.DTOs;
using Cfo.Cats.Application.Features.ManagementInformation.DTOs;
using Cfo.Cats.Application.Features.ManagementInformation.Queries;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Infrastructure.Configurations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;

namespace Cfo.Cats.Server.UI.Pages.Analytics.OutcomeQualityDipSample;

public partial class Index
{
    
    private bool _isLoading;
    private DipSampleDto[] _samples = [];
    private string? _errorMessage;
    private bool _canReview;

    [CascadingParameter] public UserProfile? CurrentUser { get; set; }

    [CascadingParameter]
    private Task<AuthenticationState> AuthState { get; set; } = default!;

    [Inject] public required IOptions<OutcomeQualityDipSampleSettings> Options { get; set; }

    public ContractDto? SelectedContract { get; set; }

    public GetOutcomeQualityDipSamples.Query Query { get; set; } = new();

    private List<BreadcrumbItem> Items =>
    [
        new("Outcome Quality", "pages/analytics/outcome-quality-dip-sampling", icon: Icons.Material.Filled.Home)
    ];


    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        _canReview = (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.OutcomeQualityDipReview)).Succeeded;


        var offset = DateTime.Now.AddMonths(Options.Value.MonthOffset);

        Query = new GetOutcomeQualityDipSamples.Query
        {
            Month = offset.Month,
            Year = offset.Year
        };

        await ReloadAsync();
    }

    private async Task ReloadAsync()
    {
        try
        {
            _isLoading = true;
            _errorMessage = null;

            var result = await GetNewMediator().Send(Query);

            if (IsDisposed)
            {
                return;
            }

            if (result is { Succeeded: true, Data: not null })
            {
                _samples = result.Data.ToArray();
            }
            else
            {
                _samples = [];
                _errorMessage = result.ErrorMessage;
            }
        }
        finally
        {
            _isLoading = false;
        }
    }
}