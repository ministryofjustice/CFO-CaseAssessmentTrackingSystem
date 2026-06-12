namespace Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.ChildhoodExperiences;
public class B8() : SingleChoiceQuestion("A parent or guardian spent time in prison",
[
    Yes,
    No,
    NA
])
{
    public override string Code => nameof(B8);
    public const string Yes = "Yes";
    public const string No = "No";
    public const string NA = "PNTS";
};