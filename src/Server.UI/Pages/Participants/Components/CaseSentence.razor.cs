using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Application.Common.Configuration;
using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class CaseSentence
{
    [Parameter]
    [EditorRequired]
    public string ParticipantId { get; set; } = default!;

    [Parameter]
    [EditorRequired]
    public bool ParticipantIsActive { get; set; }

    [Parameter]
    [EditorRequired]
    public ConsentStatus ConsentStatus { get; set; } = ConsentStatus.PendingStatus;

    [CascadingParameter]
    public UserProfile CurrentUser { get; set; } = default!;

    private bool _showHmppsRecords;

    private bool IsConsentGranted => ConsentStatus == ConsentStatus.GrantedStatus;

    protected override void OnInitialized()
        => _showHmppsRecords = Config.GetFeature(FeatureSections.HmppsSentenceInformation).IsEnabledForTenant(CurrentUser?.TenantId);
}
