using Cfo.Cats.Application.Features.Payments.DTOs;
using Cfo.Cats.Application.Features.Payments.Queries;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class ParticipantPayments
{
    private bool _includeNonPayable;

    [Parameter, EditorRequired]
    public required string ParticipantId { get; set; }

    private ParticipantPaymentDto[] Payments => Data?.Items ?? [];

    protected override IQuery<Result<ParticipantPaymentsDto>> CreateQuery()
        => new GetParticipantPayments.Query
        {
            ParticipantId = ParticipantId,
            IncludeNonPayable = _includeNonPayable
        };

    private async Task OnIncludeNonPayableChanged(bool value)
    {
        _includeNonPayable = value;
        await LoadDataAsync();
    }
}
