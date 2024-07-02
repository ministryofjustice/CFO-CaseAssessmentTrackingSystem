namespace Cfo.Cats.Application.Features.Assessments.DTOs.AssessmentModelQuestions;

public class AssessmentMultipleChoiceQuestionDto : AssessmentQuestionDto
{
    public AssessmentMultipleChoiceQuestionDto(string question, string[] options) : base(question, options)
    {
    }

    public AssessmentMultipleChoiceQuestionDto(string question, string[] options, string? helperText = null)
        : base(question, options, helperText)
    {
    }

    public IEnumerable<string>? Toggles { get; set; }

    public override bool IsValid()
    {
        return Toggles is not null && Toggles.Any();
    }
}