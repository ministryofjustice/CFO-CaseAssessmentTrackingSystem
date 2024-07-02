namespace Cfo.Cats.Application.Features.Assessments.DTOs;


/// <summary>
/// Base class for the all questions
/// </summary>
public abstract class QuestionBase
{
    /// <summary>
    /// The question we are asking
    /// </summary>
    public abstract string Question { get; }
   
    /// <summary>
    /// The weight of the question
    /// </summary>
    public abstract double Weight { get; }
    
    /// <summary>
    /// Any other errata about the question.
    /// </summary>
    public abstract string? OtherInformation { get; }
    
    /// <summary>
    /// A collection of options for the answers
    /// </summary>
    public abstract QuestionResponse[] Options { get; }
    
    /// <summary>
    /// Is the answer valid
    /// </summary>
    /// <returns>True if the answer has a valid return value</returns>
    public abstract bool IsValid();

    /// <summary>
    /// Scores the question
    /// </summary>
    /// <param name="participantAge">The age of the participant who this question is asked of</param>
    /// <param name="location">The location where the assessment took place</param>
    /// <param name="sex">The sex of the participant</param>
    /// <returns></returns>
    public abstract double Score(int participantAge, AssessmentLocation location, Sex sex);
}

public record QuestionResponse (string Answer, double Percentile);