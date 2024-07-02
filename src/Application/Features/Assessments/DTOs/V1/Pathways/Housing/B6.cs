namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Housing;

public class B6() : MultipleChoiceQuestion(
    "Do you live with any of the following? (tick all that apply)",
    "If you’re in prison, then think about who you expect to live with on release.",
    [
        "Partner/spouse",
        "Other family members",
        "Own Children",
        "Other non-family members",
        "Other Children",
        "Live alone"
    ]);