using Cfo.Cats.Application.SecurityConstants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;

namespace Cfo.Cats.Server.UI.Pages.Participants;


[Authorize(Policy=SecurityPolicies.AuthorizedUser)]
public partial class ParticipantsDashboard
{
    [Inject] private IStringLocalizer<ParticipantsDashboard> L { get; set; } = default!;


    public string? Title { get; private set; }


    protected override Task OnInitializedAsync()
    {
        Title = L["Participants Dashboard"];
        return base.OnInitializedAsync();
    }
}