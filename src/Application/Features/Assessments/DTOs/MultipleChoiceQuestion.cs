namespace Cfo.Cats.Application.Features.Assessments.DTOs;

public abstract class MultipleChoiceQuestion : QuestionBase
{
    /// <summary>
    /// The answers the user has provided
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
            if (Options.Any(o => o.Answer == answer) == false)
            {
                return false;
            }
        }

        return true;
    }


}

/*public abstract class MultipleChoiceQuestion : QuestionBase
{

    /// <summary>
    /// A collection of answers the user has given.
    /// </summary>
    public IEnumerable<string>? Answers { get; set; }

    /// <summary>
    /// Checks the validity of the answers given.
    /// </summary>
    /// <returns>true if we have at least one answer, and all sections are contained in our option list, otherwise false</returns>
    public override bool IsValid() => Answers is not null
                                      && Answers.Any()
                                      && Answers.All(a => Options.Contains(a));
}*/
