using System.Text.Json.Serialization;

namespace Cfo.Cats.Application.Features.Assessments.DTOs.AssessmentModelQuestions;

public class AssessmentToggleQuestionDto : AssessmentQuestionDto
{
    public string? SelectedOption { get; set; }

    public AssessmentToggleQuestionDto(string question, string[] options) 
        : base(question, options)
    { }

    public AssessmentToggleQuestionDto(string question, string[] options, string helperText)
        : base(question, options, helperText)
    { }

    public override bool IsValid() => SelectedOption != null;
}