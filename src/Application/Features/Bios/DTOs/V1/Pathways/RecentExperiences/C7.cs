namespace Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.RecentExperiences;
public class C7() : SingleChoiceQuestion("Subject to violence or harm by another person",
[
    Yes,
    No,
    NA
])
{
    public override string Code => nameof(C7);
    public const string Yes = "Yes";
    public const string No = "No";
    public const string NA = "PNTS";
};