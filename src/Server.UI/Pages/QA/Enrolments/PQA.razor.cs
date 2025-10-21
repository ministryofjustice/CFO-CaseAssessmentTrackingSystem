using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Assessments.Queries;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Application.Features.QualityAssurance.Commands;
using Cfo.Cats.Application.Features.QualityAssurance.DTOs;
using Cfo.Cats.Application.Features.QualityAssurance.Queries;
using System.Text;
using Color = MudBlazor.Color;
using IResult = Cfo.Cats.Application.Common.Interfaces.IResult;

namespace Cfo.Cats.Server.UI.Pages.QA.Enrolments;

public partial class PQA
{
    private int CharacterCount => Command.Message?.Length ?? 0;
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
            await _form!.Validate();

            if (_form.IsValid)
            {
                var result = await GetNewMediator().Send(Command);

                if (result.Succeeded)
                {
                    var message = Command.Response switch
                    {
                        SubmitPqaResponse.PqaResponse.Accept => "Participant submitted to QA",
                        SubmitPqaResponse.PqaResponse.Return => "Participant returned to Support Worker",
                        _ => "Comment added"
                    };

                    Snackbar.Add(message, Severity.Info);
                    Navigation.NavigateTo("/pages/qa/enrolments/pqa");
                }
                else
                {
                    var message = Command.Response switch
                    {
                        SubmitPqaResponse.PqaResponse.Accept => "Failed to submit participant to QA",
                        SubmitPqaResponse.PqaResponse.Return => "Failed to return participant to support worker",
                        _ => "Failed to add Comment/Defer"
                    };

                    ShowActionFailure(message, result);
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

    private void ShowRightToWorkWarning()
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