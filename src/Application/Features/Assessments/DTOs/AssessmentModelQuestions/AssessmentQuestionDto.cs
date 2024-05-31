using System.Text.Json.Serialization;

namespace Cfo.Cats.Application.Features.Assessments.DTOs.AssessmentModelQuestions;

public abstract class AssessmentQuestionDto(string question, string[] options)
{
    public string Question { get; } = question;

    public string? HelperText { get; }

    public string[] Options { get; } = options;

    protected AssessmentQuestionDto(string question, string[] options, string helperText) : this(question, options)
    {
        HelperText = helperText;
    }
}
