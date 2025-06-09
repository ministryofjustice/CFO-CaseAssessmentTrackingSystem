using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Assessments.Queries;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Application.Features.QualityAssurance.Commands;
using Cfo.Cats.Application.Features.QualityAssurance.DTOs;
using Cfo.Cats.Application.Features.QualityAssurance.Queries;
using Color = MudBlazor.Color;
using IResult = Cfo.Cats.Application.Common.Interfaces.IResult;

namespace Cfo.Cats.Server.UI.Pages.QA.Enrolments;

public partial class PQA
{
    private MudForm? _form;
    private EnrolmentQueueEntryDto? _queueEntry;
    private ParticipantDto? _participantDto;
    private ParticipantSummaryDto? _participantSummaryDto;
    private ParticipantAssessmentDto? _latestParticipantAssessmentDto;
    private bool _saving = false;
    [Parameter] public Guid Id { get; set; }

    [CascadingParameter] public UserProfile? UserProfile { get; set; }

    private SubmitPqaResponse.Command Command { get; set; } = default!;

    private string _rtwInfo = String.Empty;
    private string _rtwIcon = String.Empty;
    private Color _rtwIconColor = Color.Transparent;
    private bool _showRightToWorkWarning = false;
    private bool _pqaResponseDisabled = false;

    

    protected override async Task OnInitializedAsync()
    {
        if (_participantDto is null)
        {
            var result = await GetNewMediator().Send(new GetPqaEntryById.Query
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

                Command = new SubmitPqaResponse.Command
                {
                    QueueEntryId = Id,
                    CurrentUser = UserProfile
                };

                _participantSummaryDto = await GetNewMediator().Send(new GetParticipantSummary.Query()
                {
                    ParticipantId = _participantDto.Id,
                    CurrentUser = UserProfile!
                });

                await SetLatestParticipantAssessment(_queueEntry.ParticipantId);
            }

            StateHasChanged();
            ShowRightToWorkWarning();
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
            await _form!.Validate().ConfigureAwait(false);
            if (_form.IsValid)
            {
                var result = await GetNewMediator().Send(Command);

                var message = Command.Response switch
                {
                    SubmitPqaResponse.PqaResponse.Accept => "Participant submitted to QA",
                    SubmitPqaResponse.PqaResponse.Return => "Participant returned to Support Worker",
                    _ => "Comment added"
                };


                if (result.Succeeded)
                {
                    Snackbar.Add(message, Severity.Info);
                    Navigation.NavigateTo("/pages/qa/enrolments/pqa");
                }
                else
                {
                    ShowActionFailure("Failed to return to submit", result);
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

    private int CharacterCount => Command.Message?.Length ?? 0;

    void ShowRightToWorkWarning()
    {
        _showRightToWorkWarning = _participantSummaryDto!.IsRightToWorkRequired
                                && _participantSummaryDto!.ConsentStatus == Domain.Common.Enums.ConsentStatus.GrantedStatus
                                && _participantSummaryDto!.HasActiveRightToWork == false;

        if (_showRightToWorkWarning)
        {
            _rtwInfo = "Right To Work required!";
            _rtwIcon = Icons.Material.Filled.Error;
            _rtwIconColor = Color.Error;

            _pqaResponseDisabled = true;
            Command.Response = SubmitPqaResponse.PqaResponse.Return;
        }
    }

}
