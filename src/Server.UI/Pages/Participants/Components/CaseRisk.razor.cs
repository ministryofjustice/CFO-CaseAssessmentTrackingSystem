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

    private bool _showArns;
    private bool _showTier;

    protected override void OnInitialized()
    {
        _showArns = Config.GetFeature(FeatureSections.ArnsRisk).IsEnabledForTenant(CurrentUser?.TenantId);
        _showTier = Config.GetFeature(FeatureSections.TierRisk).IsEnabledForTenant(CurrentUser?.TenantId);
    }
}