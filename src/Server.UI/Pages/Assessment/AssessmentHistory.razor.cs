using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Assessments.Queries;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Infrastructure.Persistence;
using DocumentFormat.OpenXml.Drawing.Charts;
using Color = MudBlazor.Color;

namespace Cfo.Cats.Server.UI.Pages.Assessment
{
    public partial class AssessmentHistory
    {
        private IEnumerable<ParticipantAssessmentDto> _participantAssessments = Enumerable.Empty<ParticipantAssessmentDto>();
        private DateTime? _consentDate;
        private ParticipantDto? _participant;

        [CascadingParameter] public UserProfile UserProfile { get; set; } = default!;

        [Parameter]
        public string Upci { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            if (!string.IsNullOrEmpty(Upci))
            {
                var query = new GetAssessmentScores.Query()
                {
                    ParticipantId = Upci
                };

                var result = await GetNewMediator().Send(query);

                if (result.Succeeded && result.Data != null)
                {
                    _participantAssessments = result.Data
                                                    .Where(pa => pa.Completed.HasValue)
                                                    .OrderByDescending(pa => pa.CreatedDate);

                    if(_participantAssessments?.Any() == true)
                    {

                        _participant = await GetNewMediator().Send(new GetParticipantById.Query()
                        {
                            Id = Upci
                        });
                        _consentDate = _participant.CalculatedConsentDate;
                    }
                }
            }
        }

        private string GetSinceConsentDate(DateTime? assessmentCompletedDate)
        {
            if (_consentDate.HasValue && assessmentCompletedDate.HasValue)
            {
                DateTime startDate = _consentDate.Value;
                DateTime endDate = assessmentCompletedDate.Value;

                int years = endDate.Year - startDate.Year;
                int months = endDate.Month - startDate.Month;
                int days = endDate.Day - startDate.Day;

                // Adjust for negative values
                if (days < 0)
                {
                    months--;
                    DateTime previousMonth = endDate.AddMonths(-1);
                    days += DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month);
                }
                if (months < 0)
                {
                    years--;
                    months += 12;
                }

                return string.Join(", ", new[]
                {
                    years > 0 ? $"{years} years" : null,
                    months > 0 || years > 0 ? $"{months} months" : null,
                    days > 0 ? $"{days} days" : null
                }.Where(s => !string.IsNullOrEmpty(s)));
            }
            return string.Empty;
        }

    }
}