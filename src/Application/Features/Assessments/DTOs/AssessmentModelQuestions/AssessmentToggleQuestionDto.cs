namespace Cfo.Cats.Application.Features.Assessments.DTOs.AssessmentModelQuestions;

public class AssessmentToggleQuestionDto : AssessmentQuestionDto
{
    public AssessmentToggleQuestionDto(string question, string[] options, string? helperText = null)
        : base(question, options, helperText)
    {
    }

    public string? SelectedOption { get; set; }

    public override bool IsValid()
    {
        return SelectedOption != null;
    }
}