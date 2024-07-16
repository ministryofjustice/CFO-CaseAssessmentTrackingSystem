namespace Cfo.Cats.Application.Features.Bio.DTOs.V1.Pathways.RecentExperiences;
public class C10() : SingleChoiceQuestion("Found a new hobby or interest",
[
    Yes,
    No,
    NA
])
{
    public const string Yes = "Yes";
    public const string No = "No";
    public const string NA = "N/A";
};