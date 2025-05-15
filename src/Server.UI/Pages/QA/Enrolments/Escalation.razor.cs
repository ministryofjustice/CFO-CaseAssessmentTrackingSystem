using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Assessments.Queries;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Application.Features.QualityAssurance.Commands;
using Cfo.Cats.Application.Features.QualityAssurance.DTOs;
using Cfo.Cats.Application.Features.QualityAssurance.Queries;
using Cfo.Cats.Server.UI.Pages.QA.Enrolments.Components;
using IResult = Cfo.Cats.Application.Common.Interfaces.IResult;

namespace Cfo.Cats.Server.UI.Pages.QA.Enrolments;

public partial class Escalation
{
    private QaExternalMessageWarning? warningMessage;
    private MudForm? _form;
    private EnrolmentQueueEntryDto? _queueEntry;
    private ParticipantDto? _participantDto;
    private ParticipantAssessmentDto? _latestParticipantAssessmentDto;

    [Parameter] public Guid Id { get; set; }

    [CascadingParameter] public UserProfile? UserProfile { get; set; }

    private SubmitEscalationResponse.Command Command { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        if (_participantDto is null)
        {
            var result = await GetNewMediator().Send(new GetEscalationEntryById.Query
            {
                Id = Id,
                CurrentUser = UserProfile
            });

            if (result.Succeeded)
            {
                _queueEntry = result.Data!;

                _participantDto = await GetNewMediator().Send(new GetParticipantById.Query
                {
                    Id = _queueEntry.ParticipantId
                });

                Command = new SubmitEscalationResponse.Command
                {
                    QueueEntryId = Id,
                    CurrentUser = UserProfile
                };
                await SetLatestParticipantAssessment(_queueEntry.ParticipantId);
            }

            StateHasChanged();
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

        bool submit = true;

        if (Command is { IsMessageExternal: true, Message.Length: > 0 })
        {
            submit = await warningMessage!.ShowAsync();
        }

        if (submit)
        {
            var result = await GetNewMediator().Send(Command);

            var message = Command.Response switch
            {
                SubmitEscalationResponse.EscalationResponse.Accept => "Participant accepted",
                SubmitEscalationResponse.EscalationResponse.Return => "Participant returned to PQA",
                _ => "Comment added"
            };


            if (result.Succeeded)
            {
                Snackbar.Add(message, Severity.Info);
                Navigation.NavigateTo("/pages/qa/servicedesk/enrolments");
            }
            else
            {
                ShowActionFailure("Failed to return to submit", result);
            }
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

    private int characterCount => Command.Message?.Length ?? 0;
}

    bool saving = false;
        try
        {
            saving = true;
        finally { saving = false; }
        }