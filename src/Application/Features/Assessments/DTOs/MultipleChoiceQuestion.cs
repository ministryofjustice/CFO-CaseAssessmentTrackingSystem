namespace Cfo.Cats.Application.Features.Assessments.DTOs;

public abstract class MultipleChoiceQuestion : QuestionBase
{
    protected MultipleChoiceQuestion()
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

    /// <summary>
    ///     An option that is mutually exclusive with all other options (e.g. "None of the above").
    ///     When set, selecting this option will automatically clear any other selected answers, and
    ///     selecting any other option will automatically clear this one.
    /// </summary>
    public virtual string? ExclusiveOption => null;

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