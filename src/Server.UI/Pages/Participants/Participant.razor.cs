using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Assessments.Queries;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Pages.Participants;

public partial class Participant
{
    [Parameter] public string Id { get; set; } = null!;

    [CascadingParameter] public UserProfile UserProfile { get; set; } = null!;

    private ParticipantSummaryDto? _participant;
    private ParticipantCascadingDetailDto? _participantCascading;
    private ParticipantAssessmentDto? _latestParticipantAssessment;

    private bool _showRightToWorkWarning;
    private readonly string _rightToWorkAlertMessage = ConstantString.RightToWorkIsRequiredMessage;

    private readonly string _notActiveInFeedAlertMessage = ConstantString.LicenceEndedWarning;

    protected override async Task OnInitializedAsync()
    {
        await Refresh(ComponentCancellationToken);
        await SetLatestParticipantAssessment(ComponentCancellationToken);
        ShowRightToWorkWarning();
    }

    private async Task SetLatestParticipantAssessment(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(Id) == false)
        {
            var query = new GetAssessmentScores.Query()
            {
                ParticipantId = Id
            };

            var result = await GetNewMediator().Send(query, cancellationToken);

            if (result is { Succeeded: true, Data: not null })
            {
                _latestParticipantAssessment = result.Data.MaxBy(pa => pa.CreatedDate);
            }
        }
    }

    private void ShowRightToWorkWarning()
    {
        _showRightToWorkWarning = _participant!.IsRightToWorkRequired
                                  && _participant!.ConsentStatus == ConsentStatus.GrantedStatus
                                  && _participant.HasActiveRightToWork == false;
    }

    private async Task Refresh(CancellationToken cancellationToken = default)
    {
        _participant = await GetNewMediator().Send(new GetParticipantSummary.Query()
        {
            ParticipantId = Id,
            CurrentUser = UserProfile
        }, cancellationToken);
        
        if (_participant is not null)
        {
            _participantCascading = new ParticipantCascadingDetailDto
            {
                Id = _participant.Id,
                FullName = _participant.ParticipantName,
                IsActive = _participant.IsActive,
                ConsentStatus = _participant.ConsentStatus,
                DateOfFirstConsent = _participant.DateOfFirstConsent
            };
        }
    }
}