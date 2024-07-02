namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Working;

public class A1() : SingleChoiceQuestion("What is your current employment status?",
    "If you’re in prison, then think about what you expect to be doing after release.",
    [
        "Do not want a job",
        "Want a job but cannot work",
        "Looking for work",
        "In a temporary job",
        "In a permanent job"
    ]);