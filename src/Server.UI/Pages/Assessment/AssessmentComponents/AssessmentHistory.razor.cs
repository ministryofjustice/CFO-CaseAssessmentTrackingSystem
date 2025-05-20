using Cfo.Cats.Application.Features.Assessments.DTOs;

namespace Cfo.Cats.Server.UI.Pages.Assessment.AssessmentComponents;

public partial class AssessmentHistory
{
    [Parameter, EditorRequired]
    public IEnumerable<ParticipantAssessmentDto> ParticipantAssessments { get; set; } = Enumerable.Empty<ParticipantAssessmentDto>();
    [Parameter, EditorRequired]
    public DateTime? ConsentDate { get; set; }

    private string GetSinceConsentDate(DateTime? assessmentCompletedDate)
    {
        if (ConsentDate.HasValue && assessmentCompletedDate.HasValue)
        {
            DateTime startDate = ConsentDate.Value;
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