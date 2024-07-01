using Cfo.Cats.Application.Features.Assessments.DTOs.AssessmentModelQuestions;

namespace Cfo.Cats.Application.Features.Assessments.DTOs;

public class AssessmentBuilder
{
    private readonly AssessmentDto _assessment;
    private AssessmentPathwayDto? _currentPathway = null;
    
    public AssessmentBuilder()
    {
        _assessment = new AssessmentDto();
    }

    public AssessmentBuilder AddPathway(Pathway pathway)
    {
        _currentPathway = new AssessmentPathwayDto()
        {
            Pathway = pathway
        };
        _assessment.AddPathway(_currentPathway);
        return this;
    }

    public AssessmentBuilder WithMultipleChoiceQuestion(string text, string[] options)
    {
        if (_currentPathway is null)
        {
            throw new InvalidOperationException("Cannot call WithMultipleChoiceQuestion before AddPathway");
        }
        var questionDto = new AssessmentMultipleChoiceQuestionDto(text, options);
        _currentPathway.AddQuestion(questionDto);
        return this;
    }
    
    public AssessmentBuilder WithMultipleChoiceQuestion(string text, string helperText, string[] options)
    {
        if (_currentPathway is null)
        {
            throw new InvalidOperationException("Cannot call WithMultipleChoiceQuestion before AddPathway");
        }
        var questionDto = new AssessmentMultipleChoiceQuestionDto(text, options, helperText);
        _currentPathway.AddQuestion(questionDto);
        return this;
    }

    public AssessmentBuilder WithToggleChoiceQuestion(string text, string[] options)
    {
        if (_currentPathway is null)
        {
            throw new InvalidOperationException("Cannot call WithMultipleChoiceQuestion before AddPathway");
        }

        var questionDto = new AssessmentToggleQuestionDto(text, options);
        _currentPathway.AddQuestion(questionDto);
        return this;
    }
    
    public AssessmentBuilder WithToggleChoiceQuestion(string text, string helpText, string[] options)
    {
        if (_currentPathway is null)
        {
            throw new InvalidOperationException("Cannot call WithMultipleChoiceQuestion before AddPathway");
        }

        var questionDto = new AssessmentToggleQuestionDto(text, options, helpText);
        _currentPathway.AddQuestion(questionDto);
        return this;
    }

    public AssessmentBuilder WithAgreementQuestion(string text, string helpText)
    {
        if (_currentPathway is null)
        {
            throw new InvalidOperationException("Cannot call WithAgreementQuestion before AddPathway");
        }
        var questionDto = new AssessmentToggleQuestionDto(text, [
            "Strongly disagree",
            "Disagree",
            "Neither",
            "Agree",
            "Strongly agree",
        ], helpText);
        _currentPathway.AddQuestion(questionDto);
        return this;
    }
    
    public AssessmentBuilder WithAgreementQuestion(string text)
    {
        if (_currentPathway is null)
        {
            throw new InvalidOperationException("Cannot call WithAgreementQuestion before AddPathway");
        }
        var questionDto = new AssessmentToggleQuestionDto(text, [
            "Strongly disagree",
            "Disagree",
            "Neither",
            "Agree",
            "Strongly agree",
        ]);
        _currentPathway.AddQuestion(questionDto);
        return this;
    }

    public AssessmentBuilder WithFeelingQuestion(string text)
    {
        if (_currentPathway is null)
        {
            throw new InvalidOperationException("Cannot call WithAgreementQuestion before AddPathway");
        }
        var questionDto = new AssessmentToggleQuestionDto(text, [
            "Not at all",
            "Only occasionally",
            "Sometimes",
            "Often",
            "Most or all the time",
        ]);
        _currentPathway.AddQuestion(questionDto);
        return this;
    }
    
    public AssessmentBuilder WithYesNoQuestion(string text, string? helpText = null)
    {
        if (_currentPathway is null)
        {
            throw new InvalidOperationException("Cannot call WithMultipleChoiceQuestion before AddPathway");
        }
        var questionDto = new AssessmentToggleQuestionDto(text, ["Yes","No"], helpText);
        _currentPathway.AddQuestion(questionDto);
        return this;
    }

    public AssessmentDto Build()
    {
        return _assessment;
    }



}
