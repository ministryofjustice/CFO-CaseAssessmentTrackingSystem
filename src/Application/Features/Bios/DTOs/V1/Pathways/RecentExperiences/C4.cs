namespace Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.RecentExperiences;
public class C4() : SingleChoiceQuestion("Started a new personal relationship",
[
    Yes,
    No,
    NA
])
{
    public override string Code => nameof(C4);
    public const string Yes = "Yes";
    public const string No = "No";
    public const string NA = "PNTS";
};