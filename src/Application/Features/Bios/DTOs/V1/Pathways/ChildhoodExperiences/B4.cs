namespace Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.ChildhoodExperiences;
public class B4() : SingleChoiceQuestion("Had a difficult childhood",
[
    Yes,
    No,
    NA
])
{
    public override string Code => nameof(B4);
    public const string Yes = "Yes";
    public const string No = "No";
    public const string NA = "PNTS";
};