namespace Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.RecentExperiences;
public class C6() : SingleChoiceQuestion("Subject to verbal abuse or harassment",
[
    Yes,
    No,
    NA
])
{
    public override string Code => nameof(C6);
    public const string Yes = "Yes";
    public const string No = "No";
    public const string NA = "PNTS";
};