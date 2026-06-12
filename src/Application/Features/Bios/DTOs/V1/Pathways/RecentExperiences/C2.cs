namespace Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.RecentExperiences;
public class C2() : SingleChoiceQuestion("Got engaged, married, entered a civil partnership",
[
    Yes,
    No,
    NA
])
{
    public override string Code => nameof(C2);
    public const string Yes = "Yes";
    public const string No = "No";
    public const string NA = "PNTS";
};