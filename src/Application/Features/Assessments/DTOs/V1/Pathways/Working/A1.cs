namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Working;

public class A1() : SingleChoiceQuestion("What is your current employment status?",
"If you’re in prison, then think about what you expect to be doing after release.",
[
    DoNotWantAJob,
    WantAJobButCannotWork,
    LookingForWork,
    InTemporaryJob,
    InPermanentJob
])
{
    public const string DoNotWantAJob = "Do not want a job";
    public const string WantAJobButCannotWork = "Want a job but cannot work";
    public const string LookingForWork = "Looking for work";
    public const string InTemporaryJob = "In a temporary job";
    public const string InPermanentJob = "In a permanent job";
}