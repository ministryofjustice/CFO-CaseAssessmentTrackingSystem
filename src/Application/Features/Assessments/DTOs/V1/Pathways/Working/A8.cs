namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Working;

public class A8() : SingleChoiceQuestion(
"Do you own and use a smart phone?",
"If you’re in prison, then think about whether you expect to have a smart phone on release.",
[
    Yes,
    YesStruggleToAffordData,
    No
]
)
{
    public const string Yes = "Yes";
    public const string YesStruggleToAffordData = "Yes, but I struggle to afford data";
    public const string No = "No";
};
