using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Assessments.Queries;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Infrastructure.Constants;
using Humanizer;
using Color = MudBlazor.Color;

namespace Cfo.Cats.Server.UI.Pages.Participants
{
    public partial class Participant
    {
        [Parameter] public string Id { get; set; } = default!;

        [CascadingParameter] public UserProfile UserProfile { get; set; } = default!;

        private ParticipantSummaryDto? _participant = null;
        private ParticipantAssessmentDto? _latestParticipantAssessment = null;

        private string _riskInfo = String.Empty;
        private string _riskIcon = String.Empty;
        private Color _riskIconColor = Color.Transparent;

        private string _priDueToolTip = String.Empty;
        private string _priDueIcon = String.Empty;
        private Color _priDueIconColor = Color.Transparent;

        private bool _showRightToWorkWarning = false;
        private string _rightToWorkAlertMessage = ConstantString.RightToWorkIsRequiredMessage;

        private string _assessmentInfo = String.Empty;
        private string _assessmentIcon = String.Empty;
        private Color _assessmentIconColor = Color.Transparent;


        private string _bioInfo = String.Empty;
        private string _bioIcon = String.Empty;
        private Color _bioIconColor = Color.Transparent;

        private string _notActiveInFeedAlertMessage = ConstantString.NotActiveInFeedMessage;

        protected override async Task OnInitializedAsync()
        {
            var transaction = SentrySdk.StartTransaction("Participant", "page.load");
            SentrySdk.ConfigureScope(scope => scope.Transaction = transaction);

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

            try
            {
                await Refresh(cts.Token);
                await SetLatestParticipantAssessment(cts.Token);
                SetRiskDueWarning();
                ShowRightToWorkWarning();
                SetPriDueWarning(); 
                SetAssessmentDueWarning();
                SetBioDueWarning();
                
            }
            catch (OperationCanceledException)
            {
                transaction.Finish(SpanStatus.DeadlineExceeded);
                SentrySdk.CaptureMessage("Page initialization timed out");
                throw;
            }
            catch (Exception ex)
            {
                transaction.Finish(SpanStatus.InternalError);
                SentrySdk.CaptureException(ex);
                throw;
            }
            finally
            {
                if (transaction.IsFinished == false)
                {
                    transaction.Finish();
                }
                SentrySdk.ConfigureScope(scope => scope.Transaction = null);
            }

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

        void ShowRightToWorkWarning()
        {
            _showRightToWorkWarning = _participant!.IsRightToWorkRequired
                                      && _participant!.ConsentStatus == ConsentStatus.GrantedStatus
                                      && _participant.HasActiveRightToWork == false;
        }

        void SetRiskDueWarning()
        {
            if (_participant!.RiskDue.HasValue)
            {
                var datePart = _participant.RiskDue.Value.Date;

                _riskInfo = $"Due {datePart.Humanize()}";

                int dueInDays = _participant!.RiskDueInDays!.Value;
                switch (dueInDays)
                {
                    case <= 0:
                        _riskIcon = Icons.Material.Filled.Error;
                        _riskIconColor = Color.Error;
                        break;
                    case <= 14:
                        _riskIcon = Icons.Material.Filled.Warning;
                        _riskIconColor = Color.Warning;
                        break;
                }
            }
        }

        void SetPriDueWarning()
        {
            PriSummaryDto? latestPRI = _participant!.LatestPri;

            _priDueIcon = String.Empty;
            _priDueIconColor = Color.Transparent;
            if (latestPRI is null)
            {
                _priDueToolTip = _participant.LocationType switch
                {
                    { IsCustody: true, IsMapped: true } => "No PRI has been created.",
                    { IsCommunity: true } => "Not available in the community.",
                    _ => "Not available in this location."
                };
            }
            else
            {
                if (latestPRI.Status == PriStatus.Abandoned)
                {
                    _priDueToolTip = "Pre-Release Inventory has been Abandoned.";
                }
                else if (latestPRI.Status == PriStatus.Completed)
                {
                    _priDueToolTip = "Pre-Release Inventory has been Completed.";
                }
                else
                {
                    if (latestPRI.TTGDueDate.HasValue)
                    {
                        DateOnly priTTGDueDate = latestPRI.TTGDueDate.Value;
                        _priDueToolTip = String.Format(ConstantString.PriTTGDueWarningToolTip, priTTGDueDate.Humanize());

                        if (latestPRI.IsFinalTTGWarningApplicable)
                        {
                            _priDueIcon = Icons.Material.Filled.Error;
                            _priDueIconColor = Color.Error;
                        }
                        else if (latestPRI.IsFirstTTGWarningApplicable)
                        {
                            _priDueIcon = Icons.Material.Filled.Warning;
                            _priDueIconColor = Color.Warning;
                        }
                    }
                    else if (_participant.LocationType.IsCommunity)
                    {
                        _priDueToolTip = ConstantString.PriNoActualReleaseDateWarning;
                        _priDueIcon = Icons.Material.Outlined.Info;
                        _priDueIconColor = Color.Warning;
                    }
                }
            }
        }

        private async Task Refresh(CancellationToken cancellationToken = default)
        {
            _participant = await GetNewMediator().Send(new GetParticipantSummary.Query()
            {
                ParticipantId = Id,
                CurrentUser = UserProfile
            }, cancellationToken);
        }

        void SetAssessmentDueWarning()
        {
            var _latestAssessment = _participant!.Assessments is []
                ? null
                : _participant.Assessments.OrderByDescending(a => a.AssessmentDate)
                    .First();

            if (_latestAssessment is null)
            {
                _assessmentInfo = "No assessment has been created.";
                _assessmentIcon = String.Empty;
                _assessmentIconColor = Color.Transparent;
            }
            else if (_latestAssessment.Completed is null)
            {
                _assessmentInfo = "Assessment not completed";
                _assessmentIcon = Icons.Material.Filled.Warning;
                _assessmentIconColor = Color.Warning;
            }
            else
            {
                DateOnly _assessmentDueDate = DateOnly.FromDateTime(_latestAssessment.Completed.Value.AddMonths(3));
                _assessmentInfo = $"Due {_latestAssessment.Completed.Value.AddMonths(3).Humanize()}";

                int _assessmentDueInDays = _assessmentDueDate.DayNumber - DateOnly.FromDateTime(DateTime.UtcNow.Date).DayNumber;
                switch (_assessmentDueInDays)
                {
                    case var _ when _assessmentDueInDays <= 0:
                        _assessmentIcon = Icons.Material.Filled.Error;
                        _assessmentIconColor = Color.Error;
                        break;
                    case var _ when _assessmentDueInDays >= 0 && _assessmentDueInDays <= 14:
                        _assessmentIcon = Icons.Material.Filled.Warning;
                        _assessmentIconColor = Color.Warning;
                        break;
                }
            }
        }

        void SetBioDueWarning()
        {
            if (_participant!.BioDue.HasValue)
            {
                var datePart = _participant.BioDue.Value.Date;

                _bioInfo = $"Due {datePart.Humanize()}";

                int _dueInDays = _participant!.BioDueInDays!.Value;
                switch (_dueInDays)
                {
                    case var _ when _dueInDays <= 0:
                        _bioIcon = Icons.Material.Filled.Error;
                        _bioIconColor = Color.Error;
                        break;
                    case var _ when _dueInDays >= 0 && _dueInDays <= 14:
                        _bioIcon = Icons.Material.Filled.Warning;
                        _bioIconColor = Color.Warning;
                        break;
                }
            }
        }
    }
}