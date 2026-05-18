namespace Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.RecentExperiences;
public class C8() : SingleChoiceQuestion("Lost your job",
[
    Yes,
    No,
    NA
])
{
    public override string Code => nameof(C8);
    public const string Yes = "Yes";
    public const string No = "No";
    public const string NA = "PNTS";
};