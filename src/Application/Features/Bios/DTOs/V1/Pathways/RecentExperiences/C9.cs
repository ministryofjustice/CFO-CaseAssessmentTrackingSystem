namespace Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.RecentExperiences;
public class C9() : SingleChoiceQuestion("Started a new job",
[
    Yes,
    No,
    NA
])
{
    public override string Code => nameof(C9);
    public const string Yes = "Yes";
    public const string No = "No";
    public const string NA = "PNTS";
};