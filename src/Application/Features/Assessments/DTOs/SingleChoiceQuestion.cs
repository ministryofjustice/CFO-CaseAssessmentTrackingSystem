using Cfo.Cats.Application.Features.Assessments.Exceptions;

namespace Cfo.Cats.Application.Features.Assessments.DTOs;

/// <summary>
/// An implementation of <c>QuestionBase</c> that only allows a
/// single answer
/// </summary>
public abstract class SingleChoiceQuestion : QuestionBase
{
    /// <summary>
    /// The answer the user has provided
    /// </summary>
    public string? Answer { get; set; }

    /// <summary>
    /// Checks the validity of the answer given.
    /// </summary>
    /// <returns>true if the answer has been entered, and is contained in our option list, otherwise false</returns>
    public override bool IsValid() => string.IsNullOrEmpty(Answer) == false
                                      && Options.Any(a => a.Answer == Answer);

    /// <summary>
    /// Default score for a single choice question.
    /// </summary>
    /// <param name="participantAge">The age of the participant</param>
    /// <param name="location">The location where the objective took place</param>
    /// <param name="sex">The sex of the participant</param>
    /// <returns>The percentile of the answer raised to the power of the weight of the pathway.</returns>
    public override double Score(int participantAge, AssessmentLocation location, Sex sex)
    {
        // map the response to the answer
        var response = Options.FirstOrDefault(a => a.Answer == this.Answer);

        if (response == null)
        {
            throw new InvalidAnswerException(Question, Answer ?? string.Empty);
        }

        var result = Math.Pow(response.Percentile, Weight);
        return Math.Round(result, 5);
    }
    
}