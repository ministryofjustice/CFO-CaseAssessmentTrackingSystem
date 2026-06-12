namespace Cfo.Cats.Application.Features.Bios.DTOs;

public class Bio
{
    public required Guid Id { get; set; }
    public required string ParticipantId { get; set; }
    public required PathwayBase[] Pathways { get; set; }

    /// <summary>
    /// Populates question answers from tabular storage.
    /// For each question across all pathways, the matching answers are looked up
    /// by <see cref="QuestionBase.Code"/> and applied to <see cref="SingleChoiceQuestion.Answer"/>
    /// or <see cref="MultipleChoiceQuestion.Answers"/> as appropriate.
    /// </summary>
    public Bio WithAnswers(ILookup<string, string> answers)
    {
        foreach (var pathway in Pathways)
        {
            foreach (var question in pathway.Questions())
            {
                if (question is SingleChoiceQuestion single)
                {
                    var questionAnswers = answers[question.Code].ToList();
                    single.Answer = questionAnswers.FirstOrDefault();
                }
            }
        }
        return this;
    }
}