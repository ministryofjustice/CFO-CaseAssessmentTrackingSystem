using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.Assessments.DTOs;

/// <summary>
///     Base class for the all questions
/// </summary>
public abstract partial class QuestionBase
{

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected QuestionBase()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    protected QuestionBase(string question, string[] options)
    {
        this.Question = question;
        this.Options = options;
    }

    protected QuestionBase(string question, string otherInformation, string[] options)
        : this(question, options)
    {
        this.OtherInformation = otherInformation;
    }

    /// <summary>
    ///     The question we are asking
    /// </summary>
    [JsonIgnore]
    public string Question { get; }

    /// <summary>
    ///     Any other errata about the question.
    /// </summary>
    [JsonIgnore]
    public string? OtherInformation { get; }

    /// <summary>
    ///     A collection of options for the answers
    /// </summary>
    [JsonIgnore]
    public string[] Options { get; }

    /// <summary>
    ///     Is the answer valid
    /// </summary>
    /// <returns>True if the answer has a valid return value</returns>
    public abstract bool IsValid();
}

