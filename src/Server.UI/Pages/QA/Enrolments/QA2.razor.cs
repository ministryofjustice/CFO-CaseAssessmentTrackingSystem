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

public partial class QA2
{
    private QaExternalMessageWarning? warningMessage;
    private MudForm? _form;
    private EnrolmentQueueEntryDto? _queueEntry = null;
    private ParticipantDto? _participantDto = null;
    private ParticipantAssessmentDto? _latestParticipantAssessmentDto;
    private bool _saving = false;
    [CascadingParameter]
    public UserProfile? UserProfile { get; set; } = null!;

    private SubmitQa2Response.Command Command { get; set; } = default!;

    private async Task GetQueueItem()
    {
        GrabQa2Entry.Command command = new GrabQa2Entry.Command()
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

            Command = new SubmitQa2Response.Command()
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
        try
        {
            _saving = true;
            await _form!.Validate();

            if (_form.IsValid is false)
            {
                return;
            }

            bool submit = true;

            if (Command is { IsMessageExternal: true, Message.Length: > 0 })
            {
                submit = await warningMessage!.ShowAsync();
            }

            if (submit)
            {
                var result = await GetNewMediator().Send(Command);

                if (result.Succeeded)
                {
                    Snackbar.Add("Participant submitted", Severity.Info);
                    Navigation.NavigateTo("/pages/qa/enrolments/qa2", true);
                }
                else
                {
                    ShowActionFailure("Failed to submit", result);
                }
            }
        }
        finally { _saving = false; }
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

    private int characterCount => Command.Message?.Length ?? 0;

    private void UpdateCharacterCount(ChangeEventArgs args)
    {
        Command.Message = args?.Value?.ToString() ?? string.Empty;
    }

}
