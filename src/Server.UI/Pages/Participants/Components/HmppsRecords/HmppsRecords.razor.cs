using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Application.Features.Participants.Queries;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components.HmppsRecords;

public partial class HmppsRecords
{
    [Parameter]
    [EditorRequired]
    public string ParticipantId { get; set; } = default!;

    private IHmppsExternalApiService? _service;
    private (ExternalIdentifierType Type, string Value)[] _records = [];
    private bool _loading = true;

    protected override async Task OnInitializedAsync()
    {
        // The HMPPS External API client is only registered when the feature is enabled.
        _service = ScopedServices.GetService<IHmppsExternalApiService>();

        if (_service is null)
        {
            return;
        }

        var identifiers = await Service.Send(new GetExternalIdentifiers.Query
        {
            ParticipantId = ParticipantId
        });

        _records = identifiers
            .Select(i => ExternalIdentifierType.TryFromName(i.Type, out var type) ? (Type: type, i.Value) : default)
            .Where(r => r.Type == ExternalIdentifierType.NomisNumber || r.Type == ExternalIdentifierType.Crn)
            .OrderBy(r => r.Type.DisplayOrderPriority)
            .ThenBy(r => r.Value)
            .ToArray();

        _loading = false;
    }
}
