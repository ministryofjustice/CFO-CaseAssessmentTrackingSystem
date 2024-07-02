using System.Diagnostics.Contracts;
using System.Text.Json.Serialization;
using Cfo.Cats.Application.Features.Assessments.DTOs.AssessmentModelQuestions;
using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Application.Features.Assessments.DTOs;

public class AssessmentPathwayDto
{
    public List<AssessmentQuestionDto> Questions { get; set; } = new();
    
    public required Pathway Pathway { get; set; }

    public AssessmentToggleQuestionDto[] ToggleQuestions => Questions.OfType<AssessmentToggleQuestionDto>().ToArray();
    public AssessmentMultipleChoiceQuestionDto[] CheckboxQuestions => Questions.OfType<AssessmentMultipleChoiceQuestionDto>().ToArray();
    public AssessmentQuestionDto LastQuestion => Questions.Last();


    public void AddQuestion(AssessmentQuestionDto questionDto)
    {
        Questions.Add(questionDto);
    }
}