namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Working;

public sealed class WorkingPathway
    : PathwayBase
{
    public WorkingPathway()
    {
        Questions =
        [
            A1 = new A1(),
            A2 = new A2(),
            A3 = new A3(),
            A4 = new A4(),
            A5 = new A5(),
            A6 = new A6(),
            A7 = new A7(),
            A8 = new A8(),
            A9 = new A9(),
            A10 = new A10()
        ];
    }

    public override string Title => "Wellbeing & Mental Health";
    public override double Constant => 0.79225;
    public override string Icon => CatsIcons.Working;
    public A1 A1 { get; private set; }
    public A2 A2 { get; private set; }
    public A3 A3 { get; private set; }
    public A4 A4 { get; private set; }
    public A5 A5 { get; private set; }
    public A6 A6 { get; private set; }
    public A7 A7 { get; private set; }
    public A8 A8 { get; private set; }
    public A9 A9 { get; private set; }
    public A10 A10 { get; private set; }
    protected override IEnumerable<double> GetPercentiles(int age, AssessmentLocation location, Sex sex)
    {
        // get all the scores for each answer
        double[] percentiles =
        [
            PercentileA1(),
            PercentileA2(),
            PercentileA4(),
            PercentileA5(),
            PercentileA6(),
            PercentileA7(age),
            PercentileA8(),
            PercentileA9(location),
        ];
        return percentiles;
    }
    private double PercentileA9(AssessmentLocation location)
    {
        const double weight = 0.08;
        var percentile = (location.Name, A9.Answer) switch
        {
            (AssessmentLocation.EastMidlands, A7.No) => 0.18,
            (AssessmentLocation.EastOfEngland, A7.No) => 0.15,
            (AssessmentLocation.London, A7.No) => 0.45,
            (AssessmentLocation.NorthEast, A7.No) => 0.29,
            (AssessmentLocation.SouthEast, A7.No) => 0.16,
            (AssessmentLocation.SouthWest, A7.No) => 0.15,
            (AssessmentLocation.WestMidlands, A7.No) => 0.26,
            (AssessmentLocation.YorkshireAndHumberside, A7.No) => 0.23,
            (_, A7.Yes) => 1,
            _ => throw new ArgumentException("Invalid answer")
        };
        return percentile.PowerRound(weight);
    }
    private double PercentileA8()
    {
        const double weight = 0.05;
        var percentile = A8.Answer switch
        {
            A8.Yes => 1,
            A8.YesStruggleToAffordData => 0.23,
            A8.No => 0.16,
            _ => throw new ArgumentException("Invalid answer")
        };
        return percentile.PowerRound(weight);
    }
    private double PercentileA7(int age)
    {
        const double weight = 0.03;
        var percentile = (age, A7.Answer) switch
        {
            (< 20, A7.No) => 0.06,
            (>= 20 and <= 24, A7.No) => 0.06,
            (<= 29, A7.No) => 0.06,
            (<= 34, A7.No) => 0.06,
            (<= 39, A7.No) => 0.09,
            (<= 44, A7.No) => 0.09,
            (<= 49, A7.No) => 0.1,
            (<= 54, A7.No) => 0.1,
            (<= 59, A7.No) => 0.16,
            (<= 64, A7.No) => 0.16,
            (_, A7.No) => 0.29,
            (_, A7.Yes) => 1,
            _ => throw new ArgumentException("Invalid answer")
        };
        return percentile.PowerRound(weight);
    }
    private double PercentileA6()
    {
        const double weight = 0.08;
        var percentile = A6.Answer switch
        {
            A6.YesUnaided => 1,
            A6.YesWithHelp => 0.01,
            A6.No => 0.001,
            _ => throw new ArgumentException("Invalid answer")
        };
        return percentile.PowerRound(weight);
    }

    private double PercentileA1()
    {
        const double weight = 0.5;
        var percentile = A1.Answer switch
        {
            A1.InPermanentJob => 1,
            A1.InTemporaryJob => 0.29,
            A1.LookingForWork => 0.25,
            A1.WantAJobButCannotWork => 0.21,
            A1.DoNotWantAJob => 0.17,
            _ => throw new ArgumentException("Invalid answer")
        };
        return percentile.PowerRound(weight);
    }

    private double PercentileA2()
    {
        const double weight = 0.15;
        var percentile = A2.Answer switch
        {
            A2.CurrentlyWorking => 1,
            A2.InTheLastYear => 0.25,
            A2.OverAYearAgo => 0.22,
            A2.NeverWorked => 0.06,
            _ => throw new ArgumentException("Invalid answer")
        };
        return percentile.PowerRound(weight);
    }

    private double PercentileA4()
    {
        const double weight = 0.09;
        var percentile = A4.Answer switch
        {
            A4.AtLeastOncePerMonth => 1,
            A4.LessThanOncePerMonth => 0.82,
            A4.NoCannot => 0.7,
            A4.NoPreferNot => 0.25,
            _ => throw new ArgumentException("Invalid answer")
        };
        return percentile.PowerRound(weight);
    }

    private double PercentileA5()
    {
        const double weight = 0.02;
        var percentile = A5.Answer switch
        {
            A5.Yes => 1,
            A5.No => 0.01,
            _ => throw new ArgumentException("Invalid answer")
        };
        return percentile.PowerRound(weight);
    }
}




