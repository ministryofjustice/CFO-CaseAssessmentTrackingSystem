using System.Text.Json.Serialization;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Housing;

namespace Cfo.Cats.Application.Features.Assessments.DTOs;

public abstract partial class MultipleChoiceQuestion : QuestionBase
{
    protected MultipleChoiceQuestion()
        :base()
    {
    }

    protected MultipleChoiceQuestion(string question, string[] options) : base(question, options)
    {
    }

    protected MultipleChoiceQuestion(string question, string otherInformation, string[] options) : base(question, otherInformation, options)
    {
    }
    
    /// <summary>
    ///     The answers the user has provided
    /// </summary>
    public IEnumerable<string>? Answers { get; set; }

    public override bool IsValid()
    {
        if (Answers is null || Answers.Any() == false)
        {
            return false;
        }

        foreach (var answer in Answers)
        {
            if (Options.Any(o => o == answer) == false)
            {
                return false;
            }
        }

        return true;
    }

}