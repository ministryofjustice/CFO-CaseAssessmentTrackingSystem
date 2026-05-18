namespace Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.RecentExperiences;
public class C10() : SingleChoiceQuestion("Found a new hobby or interest",
[
    Yes,
    No,
    NA
])
{
    public override string Code => nameof(C10);
    public const string Yes = "Yes";
    public const string No = "No";
    public const string NA = "PNTS";
};