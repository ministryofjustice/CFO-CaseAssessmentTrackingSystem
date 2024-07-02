namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Housing;

public class B4() : SingleChoiceQuestion(
    "How satisfied are you with your current housing?",
    "If you’re in prison, then think about your expected housing on release.",
    [
        "Very dissatisfied",
        "Slightly dissatisfied",
        "Neither satisfied or dissatisfied",
        "Fairly satisfied",
        "Very satisfied",
    ]);