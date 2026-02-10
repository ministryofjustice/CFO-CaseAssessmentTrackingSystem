using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Assessments.Queries;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Application.Features.QualityAssurance.Commands;
using Cfo.Cats.Application.Features.QualityAssurance.DTOs;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Server.UI.Pages.QA.Enrolments.Components;
using System.Text;
using IResult = Cfo.Cats.Application.Common.Interfaces.IResult;

namespace Cfo.Cats.Server.UI.Pages.QA.Enrolments;

public partial class QA2
{
    private int characterCount => Command.Message?.Length ?? 0; 
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

            if (Command is { MessageToProvider.Length: > 0 })
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
                    ShowActionFailure("Failed to submit participant", result);
                }
            }
        }
        finally { _saving = false; }
    }

    private void ShowActionFailure(string title, IResult result)
    {
        var html = new StringBuilder();

        html.Append($"<div>");
        html.Append($"<h2>{title}</h2>");
        html.Append($"<ul>");
        foreach (var e in result.Errors)
        {
            html.Append($"<li>{e}</li>");
        }
        html.Append($"</ul>");
        html.Append($"</div>");

        RenderFragment content = builder =>
        {
            builder.AddMarkupContent(0, html.ToString());
        };

        Snackbar.Add(content, Severity.Error, options =>
        {
            options.RequireInteraction = true;
            options.SnackbarVariant = Variant.Text;
        });
    }
    private void OnResponseChanged()
    {
        if (Command.Response == SubmitQa2Response.Qa2Response.Accept)
        {
            Command.FeedbackType = null;
            Command.MessageToProvider = string.Empty;
        }
        else if (Command.Response == SubmitQa2Response.Qa2Response.Return)
        {
            Command.FeedbackType = FeedbackType.Returned;
            Command.MessageToProvider = string.Empty;
        }
        else if (Command.Response == SubmitQa2Response.Qa2Response.Escalate)
        {
            Command.FeedbackType = null;
            Command.MessageToProvider = string.Empty;
        }
    }    
}