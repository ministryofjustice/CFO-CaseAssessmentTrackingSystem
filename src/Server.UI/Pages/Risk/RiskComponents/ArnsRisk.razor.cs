using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;

namespace Cfo.Cats.Server.UI.Pages.Risk.RiskComponents;

public partial class ArnsRisk
{
    [Parameter]
    [EditorRequired]
    public string ParticipantId { get; set; } = default!;

    private IHmppsExternalApiService? _service;

    private ExternalIdentifierDto[] _identifiers = [];

    protected override async Task OnInitializedAsync()
    {
        // The HMPPS External API client is only registered when the feature is enabled.
        // Resolve it optionally so this component degrades gracefully when the feature is gated off.
        _service = ScopedServices.GetService<IHmppsExternalApiService>();

        if (_service is null)
        {
            return;
        }

        _identifiers = await Service.Send(new GetExternalIdentifiers.Query
        {
            ParticipantId = ParticipantId
        });
    }
}
