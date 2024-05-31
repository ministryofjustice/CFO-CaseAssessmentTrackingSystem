namespace Cfo.Cats.Application.Features.Assessments.DTOs.AssessmentModelQuestions;

public class AssessmentMultipleChoiceQuestionDto : AssessmentQuestionDto
{
    public bool[] Toggles { get; set; } = [];

    public bool AnyToggleSelected => !Toggles.All(toggle => false);

    public AssessmentMultipleChoiceQuestionDto(string question, string[] options) : base(question, options)
    { }

    public AssessmentMultipleChoiceQuestionDto(string question, string[] options, string helperText)
        : base(question, options, helperText)
    { }
}