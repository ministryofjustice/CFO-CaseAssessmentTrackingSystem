namespace Cfo.Cats.Application.Features.Assessments.DTOs;

public class Assessment
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
    public Assessment WithAnswers(ILookup<string, string> answers)
    {
        foreach (var pathway in Pathways)
        {
            foreach (var question in pathway.Questions())
            {
                var questionAnswers = answers[question.Code].ToList();
                switch (question)
                {
                    case SingleChoiceQuestion single:
                        single.Answer = questionAnswers.FirstOrDefault();
                        break;
                    case MultipleChoiceQuestion multi:
                        multi.Answers = questionAnswers.Count > 0 ? questionAnswers : null;
                        break;
                }
            }
        }
        return this;
    }
}