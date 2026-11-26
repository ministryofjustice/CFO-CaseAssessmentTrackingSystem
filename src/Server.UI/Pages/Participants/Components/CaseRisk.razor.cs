using Cfo.Cats.Application.Common.Configuration;
using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;
public partial class CaseRisk
{
    [Parameter]
    [EditorRequired]
    public string ParticipantId { get; set; } = default!;

    [CascadingParameter]
    public UserProfile CurrentUser { get; set; } = default!;

    private bool _showHmppsRisk;

    protected override void OnInitialized()
        => _showHmppsRisk = Config.GetFeature(FeatureSections.HmppsRisk).IsEnabledForTenant(CurrentUser?.TenantId);
}