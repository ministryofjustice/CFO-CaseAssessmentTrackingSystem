namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Housing;

public sealed class HousingPathway : PathwayBase
{
    public HousingPathway()
    {
        Questions =
        [
            B1 = new B1(),
            B2 = new B2(),
            B3 = new B3(),
            B4 = new B4(),
            B5 = new B5(),
            B6 = new B6()
        ];
    }
    public override string Title => "Housing";
    public override double Constant => 0.87634D;
    public override string Icon => CatsIcons.Housing;
    public B1 B1 { get; private set; }
    public B2 B2 { get; private set; }
    public B3 B3 { get; private set; }
    public B4 B4 { get; private set; }
    public B5 B5 { get; private set; }
    public B6 B6 { get; private set; }
    
    protected override IEnumerable<double> GetPercentiles(int age, AssessmentLocation location, Sex sex)
    {
        double[] results =
        [
            GetHousingStatusPercentile(),
            GetHousingCosts(),
            GetFacingEvictions(),
            GetFearOfHomelessness(),
            GetHousingSatisfaction(),
            GetFeelingSafe(sex),
        ];
        return results;
    }
    private double GetFeelingSafe(Sex sex)
    {
        const double weight = 0.11;
        double percentile = (sex.Name, B5.Answer) switch {
            (Sex.Female, B5.VerySafeToWalkAloneAfterDarkInLocalArea) => 1,
            (Sex.Female, B5.FairlySafeToWalkAloneAfterDarkInLocalArea) => 0.79,
            (Sex.Female, B5.ABitUnsafeToWalkAloneAfterDarkInLocalArea) => 0.31,
            (Sex.Female, B5.VeryUnsafeToWalkAloneAfterDarkInLocalArea) => 0.09,
            (Sex.Male, B5.VerySafeToWalkAloneAfterDarkInLocalArea) => 1,
            (Sex.Male, B5.FairlySafeToWalkAloneAfterDarkInLocalArea) => 0.56,
            (Sex.Male, B5.ABitUnsafeToWalkAloneAfterDarkInLocalArea) => 0.10,
            (Sex.Male, B5.VeryUnsafeToWalkAloneAfterDarkInLocalArea) => 0.02,
            _ => throw new ApplicationException("Unknown answer")
        };
        return percentile.PowerRound(weight);
    }
    private double GetHousingSatisfaction()
    {
        const double weight = 0.24;
        double percentile = B4.Answer switch
        {
            B4.VerySatisfied => 1,
            B4.FairlySatisfied => 0.43,
            B4.NeitherSatisfiedOrDissatisfied => 0.12,
            B4.SlightlyDissatisfied => 0.07,
            B4.VeryDissatisfied => 0.03,
            _ => throw new ApplicationException("Unknown answer")
        };
        return percentile.PowerRound(weight);
    }
    private double GetFearOfHomelessness()
    {
        const double weight = 0.12;
        var percentile = B2.Answers!.Any(a => a == B2.RiskOfHomelessness)
            ? 0.02 : 1;

        return percentile.PowerRound(weight);
    }
    private double GetFacingEvictions()
    {
        const double weight = 0.01;
        var percentile = B2.Answers!.Any(a => a == B2.FacingEviction)
            ? 0.02 : 1;

        return percentile.PowerRound(weight);
    }
    private double GetHousingCosts()
    {
        const double weight = 0.02;
        var percentile = B2.Answers!.Any(a => a == B2.BehindOnRentOrMortgage)
            ? 0.09 : 1;

        return percentile.PowerRound(weight);
    }

    private double GetHousingStatusPercentile()
    {
        const double weight = 0.5;
        double percentile = B1.Answer switch
        {
            B1.HousingRentedOrOwnedByYouOrYourPartnerParentOrGuardian => 1,
            B1.TemporarilyStayingWithFamilyOrFriends => 0.01,
            B1.TemporaryOrSupportedHousing => 0.01,
            B1.SleepRough => 0.001,
            B1.ShelterHostelEmergencyHousingOrAP => 0.001,
            _ => throw new ApplicationException("Unknown answer")
        };
        return percentile.PowerRound(weight);
    }

}