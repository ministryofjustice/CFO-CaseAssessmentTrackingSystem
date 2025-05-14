using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Assessments.Queries;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Application.Features.QualityAssurance.Commands;
using Cfo.Cats.Application.Features.QualityAssurance.DTOs;
using Cfo.Cats.Server.UI.Pages.QA.Enrolments.Components;
using IResult = Cfo.Cats.Application.Common.Interfaces.IResult;

namespace Cfo.Cats.Server.UI.Pages.QA.Enrolments;

public partial class QA1
{
    private int CharacterCount => Command.Message?.Length ?? 0;
    private QaExternalMessageWarning? _warningMessage;
    private MudForm? _form;
    private EnrolmentQueueEntryDto? _queueEntry = null;
    private ParticipantDto? _participantDto = null;
    private ParticipantAssessmentDto? _latestParticipantAssessmentDto;

    [CascadingParameter]
    public UserProfile? UserProfile { get; set; } = null!;

    private SubmitQa1Response.Command Command { get; set; } = default!;

    private async Task GetQueueItem()
    {
        GrabQa1Entry.Command command = new GrabQa1Entry.Command()
        {
            CurrentUser = UserProfile!
        };

        var result = await GetNewMediator().Send(command);

        if (result.Succeeded)
        {
            _queueEntry = result.Data!;

            _participantDto = await GetNewMediator().Send(new GetParticipantById.Query()
            {
                Id = _queueEntry.ParticipantId
            });

            Command = new SubmitQa1Response.Command()
            {
                QueueEntryId = _queueEntry.Id,
                CurrentUser = UserProfile
            };

            await SetLatestParticipantAssessment(_queueEntry.ParticipantId);
        }
        else
        {
            Snackbar.Add(result.ErrorMessage, Severity.Info);
        }
    }

    protected async Task SetLatestParticipantAssessment(string participantId)
    {
        if (!string.IsNullOrEmpty(participantId))
        {
            var query = new GetAssessmentScores.Query()
            {
                ParticipantId = participantId
            };

            var result = await GetNewMediator().Send(query);

            if (result.Succeeded && result.Data != null)
            {
                _latestParticipantAssessmentDto = result.Data.MaxBy(pa => pa.CreatedDate);
            }

        }
    }

    protected async Task SubmitToQa()
    {
        await _form!.Validate().ConfigureAwait(false);

        if (_form.IsValid is false)
        {
            return;
        }

        var result = await GetNewMediator().Send(Command);

        if (result.Succeeded)
        {
            Snackbar.Add("Participant submitted to QA2", Severity.Info);
            Navigation.NavigateTo("/pages/qa/enrolments/qa1", true);
        }
        else
        {
            ShowActionFailure("Failed to submit", result);
        }

    }

    private void ShowActionFailure(string title, IResult result)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        sb.Append("<div>");
        sb.Append($"<h2>{title}</h2>");
        sb.Append("<ul>");

        foreach (var e in result.Errors)
        {
            sb.Append($"<li>{e}</li>");
        }

        sb.Append("</ul>");
        sb.Append("</div>");

        Snackbar.Add(sb.ToString(), Severity.Error, options =>
        {
            options.RequireInteraction = true;
            options.SnackbarVariant = Variant.Text;
        });
    }



    private void UpdateCharacterCount(ChangeEventArgs args)
    {
        Command.Message = args?.Value?.ToString() ?? string.Empty;
    }
}

    bool saving = false;
        try
        {
            saving = true;
        }
        finally { saving = false; }