using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Contracts.DTOs;

namespace Cfo.Cats.Server.UI.Pages.Analytics;

public partial class DipSampling
{
    [CascadingParameter] public UserProfile? CurrentUser { get; set; }

    public int Month { get; set; } = DateTime.Now.AddMonths(-4).Month;
    public int Year { get; set; } = DateTime.Now.AddMonths(-4).Year;
    public ContractDto? SelectedContract { get; set; }

    protected override Task OnInitializedAsync()
    {
        return base.OnInitializedAsync();
    }

}
