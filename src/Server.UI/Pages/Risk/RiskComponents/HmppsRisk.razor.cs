using Cfo.Cats.Application.Common.Configuration;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Server.UI.Pages.Risk.RiskComponents;

public partial class HmppsRisk : CatsComponent<ParticipantIdentifierDto[]>
{
    [Parameter]
    [EditorRequired]
    public string ParticipantId { get; set; } = default!;

    private bool _integrationEnabled;

    private string[] _crns = [];

    protected override async Task OnInitializedAsync()
    {
        _integrationEnabled = Config.IsFeatureEnabled(FeatureSections.HmppsExternalApi);

        if (_integrationEnabled)
        {
            await base.OnInitializedAsync();
        }
    }

    protected override IQuery<Result<ParticipantIdentifierDto[]>> CreateQuery() => new GetParticipantIdentifiers.Query
    {
        ParticipantId = ParticipantId
    };

    protected override void OnDataLoaded(ParticipantIdentifierDto[] data)
    {
        base.OnDataLoaded(data);

        _crns = data
            .Where(i => i.Type == ExternalIdentifierType.Crn)
            .Select(i => i.Value)
            .Distinct()
            .Order()
            .ToArray();
    }
}
