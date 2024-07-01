namespace Cfo.Cats.Application.Features.Assessments.DTOs.AssessmentModelQuestions;

public class AssessmentMultipleChoiceQuestionDto : AssessmentQuestionDto
{
    public IEnumerable<string>? Toggles { get; set; }


    public AssessmentMultipleChoiceQuestionDto(string question, string[] options) : base(question, options)
    { }

    public AssessmentMultipleChoiceQuestionDto(string question, string[] options, string? helperText = null)
        : base(question, options, helperText)
    { }

    public override bool IsValid() => Toggles is not null && Toggles.Any();
}