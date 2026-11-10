using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;

namespace Cfo.Cats.Server.UI.Components.Participants;

/// <summary>
/// This component displays key header information inside of a mud card.
/// </summary>
public partial class ParticipantDetailsCardComponent
{
    protected override IQuery<Result<ParticipantHeaderDetailsDto>> CreateQuery()
        => new GetParticipantDetails.Query()
        {
            CurrentUser = CurrentUser,
            ParticipantId = ParticipantId
        };

    private async Task ExpandEnrolmentJustification()
    {
        if (string.IsNullOrWhiteSpace(Data?.EnrolmentJustification))
        {
            return;
        }

        var parameters = new DialogParameters<EnrolmentJustificationDialog>
        {
            { dialog => dialog.Justification, Data.EnrolmentJustification }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        var dialog = await DialogService.ShowAsync<EnrolmentJustificationDialog>(
            "Enrolment Justification", parameters, options);

        await dialog.Result;
    }
}
