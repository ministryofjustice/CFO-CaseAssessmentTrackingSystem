namespace Cfo.Cats.Application.Features.Assessments.DTOs.AssessmentModelQuestions;

public abstract class AssessmentQuestionDto(string question, string[] options)
{
    protected AssessmentQuestionDto(string question, string[] options, string? helperText) : this(question, options)
    {
        HelperText = helperText;
    }

    public string Question { get; } = question;

    public string? HelperText { get; }

    public string[] Options { get; } = options;

    public abstract bool IsValid();
}