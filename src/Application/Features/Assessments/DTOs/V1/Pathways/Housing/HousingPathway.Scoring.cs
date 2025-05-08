namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Housing;

public partial class HousingPathway
{
    internal override IEnumerable<double> GetPercentiles(int age, AssessmentLocation location, Sex sex)
    {
        double[] results =
        [
            GetHousingStatusPercentile(B1).PowerRound(0.5),
            GetHousingCosts(B2).PowerRound(0.02),
            GetFacingEvictions(B2).PowerRound(0.01),
            GetFearOfHomelessness(B2).PowerRound(0.12),
            GetHousingSatisfaction(B4).PowerRound(0.24),
            GetFeelingSafe(sex, B5).PowerRound(0.11)
        ];
        return results;
    }
    internal static double GetFeelingSafe(Sex sex, B5 answer)
        => (sex.Name, answer.Answer) switch
        {
            (Sex.Female, B5.VerySafeToWalkAloneAfterDarkInLocalArea) => 1,
            (Sex.Female, B5.FairlySafeToWalkAloneAfterDarkInLocalArea) => 0.79,
            (Sex.Female, B5.ABitUnsafeToWalkAloneAfterDarkInLocalArea) => 0.31,
            (Sex.Female, B5.VeryUnsafeToWalkAloneAfterDarkInLocalArea) => 0.09,
            (Sex.Male, B5.VerySafeToWalkAloneAfterDarkInLocalArea) => 1,
            (Sex.Male, B5.FairlySafeToWalkAloneAfterDarkInLocalArea) => 0.56,
            (Sex.Male, B5.ABitUnsafeToWalkAloneAfterDarkInLocalArea) => 0.10,
            (Sex.Male, B5.VeryUnsafeToWalkAloneAfterDarkInLocalArea) => 0.02,
            (Sex.Unknown, B5.VerySafeToWalkAloneAfterDarkInLocalArea) => 1,
            (Sex.Unknown, B5.FairlySafeToWalkAloneAfterDarkInLocalArea) => 0.67,
            (Sex.Unknown, B5.ABitUnsafeToWalkAloneAfterDarkInLocalArea) => 0.20,
            (Sex.Unknown, B5.VeryUnsafeToWalkAloneAfterDarkInLocalArea) => 0.05,
            _ => throw new ApplicationException("Unknown answer")
        };
    internal static double GetHousingSatisfaction(B4 answer)
        => answer.Answer switch
        {
            B4.VerySatisfied => 1,
            B4.FairlySatisfied => 0.43,
            B4.NeitherSatisfiedOrDissatisfied => 0.12,
            B4.SlightlyDissatisfied => 0.07,
            B4.VeryDissatisfied => 0.03,
            _ => throw new ApplicationException("Unknown answer")
        };
    internal static double GetFearOfHomelessness(B2 answer)
        => answer.Answers!.Any(a => a == B2.RiskOfHomelessness)
            ? 0.02
            : 1;
    internal static double GetFacingEvictions(B2 answer)
        => answer.Answers!.Any(a => a == B2.FacingEviction)
            ? 0.02
            : 1;
    internal static double GetHousingCosts(B2 answer)
        => answer.Answers!.Any(a => a == B2.BehindOnRentOrMortgage)
            ? 0.09
            : 1;

    internal static double GetHousingStatusPercentile(B1 answer)
        => answer.Answer switch
        {
            B1.HousingRentedOrOwnedByYouOrYourPartnerParentOrGuardian => 1,
            B1.TemporarilyStayingWithFamilyOrFriends => 0.01,
            B1.TemporaryOrSupportedHousing => 0.01,
            B1.SleepRough => 0.001,
            B1.ShelterHostelEmergencyHousingOrAP => 0.001,
            _ => throw new ApplicationException("Unknown answer")
        };
}
