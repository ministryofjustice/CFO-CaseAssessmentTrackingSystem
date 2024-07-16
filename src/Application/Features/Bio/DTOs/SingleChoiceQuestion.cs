using System.Text.Json.Serialization;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Education;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.HealthAndAdditiction;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Working;

namespace Cfo.Cats.Application.Features.Bio.DTOs;

/// <summary>
///     An implementation of <c>QuestionBase</c> that only allows a
///     single answer
/// </summary>

public abstract class SingleChoiceQuestion : QuestionBase
{
    protected SingleChoiceQuestion()
    : base()
    {
    }

    protected SingleChoiceQuestion(string question, string[] options)
        : base(question, options)
    {
     
    }

    protected SingleChoiceQuestion(string question, string otherInformation, string[] options)
        : base(question, otherInformation, options)
    {
    }


    /// <summary>
    ///     The answer the user has provided
    /// </summary>
    public string? Answer { get; set; }

    /// <summary>
    ///     Checks the validity of the answer given.
    /// </summary>
    /// <returns>true if the answer has been entered, and is contained in our option list, otherwise false</returns>
    public override bool IsValid()
    {
        return string.IsNullOrEmpty(Answer) == false
               && Options.Any(a => a == Answer);
    }
}