using System.Text.Json.Serialization;
using Cfo.Cats.Application.Features.Assessments.DTOs.AssessmentModelQuestions;

namespace Cfo.Cats.Application.Features.Assessments.DTOs;

public class AssessmentPathwayDto
{
    public AssessmentQuestionDto[] Questions { get; set; } = [];
    
    public required Pathway Pathway { get; set; }

    public AssessmentToggleQuestionDto[] ToggleQuestions => Questions.OfType<AssessmentToggleQuestionDto>().ToArray();
    public AssessmentMultipleChoiceQuestionDto[] CheckboxQuestions => Questions.OfType<AssessmentMultipleChoiceQuestionDto>().ToArray();
    public AssessmentQuestionDto LastQuestion => Questions.Last();

    
}