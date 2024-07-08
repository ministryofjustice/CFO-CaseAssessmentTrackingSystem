namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Working;

public partial class WorkingPathway
{
    internal override IEnumerable<double> GetPercentiles(int age, AssessmentLocation location, Sex sex)
    {
        // get all the scores for each answer
        double[] percentiles =
        [
            GetEmploymentStatus(A1).PowerRound(0.5),
            GetEmploymentHistory(A2).PowerRound(0.15),
            GetVolunteering(A4).PowerRound(0.09),
            GetJobSearch(A5).PowerRound(0.02),
            GetWorking(A6).PowerRound(0.08),
            GetComputers(age, A7).PowerRound(0.03),
            GetConnected(A8).PowerRound(0.05),
            GetTransport(location, A10).PowerRound(0.08)
        ];
        return percentiles;
    }
    internal static double GetTransport(AssessmentLocation location, A10 answer)
        => (location.Name, answer.Answer) switch
        {
            (AssessmentLocation.EastMidlands, A7.No) => 0.18,
            (AssessmentLocation.EastOfEngland, A7.No) => 0.15,
            (AssessmentLocation.London, A7.No) => 0.45,
            (AssessmentLocation.NorthEast, A7.No) => 0.29,
            (AssessmentLocation.NorthWest, A7.No) => 0.24,
            (AssessmentLocation.SouthEast, A7.No) => 0.16,
            (AssessmentLocation.SouthWest, A7.No) => 0.15,
            (AssessmentLocation.WestMidlands, A7.No) => 0.26,
            (AssessmentLocation.YorkshireAndHumberside, A7.No) => 0.23,
            (_, A7.Yes) => 1,
            _ => throw new ArgumentException("Invalid answer")
        };
    internal static double GetConnected(A8 answer)
        => answer.Answer switch
        {
            A8.Yes => 1,
            A8.YesStruggleToAffordData => 0.23,
            A8.No => 0.16,
            _ => throw new ArgumentException("Invalid answer")
        };
    internal static double GetComputers(int age, A7 answer)
        => (age, answer.Answer) switch
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
    internal static double GetWorking(A6 answer)
        => answer.Answer switch
        {
            A6.YesUnaided => 1,
            A6.YesWithHelp => 0.01,
            A6.No => 0.001,
            _ => throw new ArgumentException("Invalid answer")
        };

    internal static double GetEmploymentStatus(A1 answer)
        => answer.Answer switch
        {
            A1.InPermanentJob => 1,
            A1.InTemporaryJob => 0.29,
            A1.LookingForWork => 0.25,
            A1.WantAJobButCannotWork => 0.21,
            A1.DoNotWantAJob => 0.17,
            _ => throw new ArgumentException("Invalid answer")
        };

    internal static double GetEmploymentHistory(A2 answer)
        => answer.Answer switch
        {
            A2.CurrentlyWorking => 1,
            A2.InTheLastYear => 0.25,
            A2.OverAYearAgo => 0.22,
            A2.NeverWorked => 0.06,
            _ => throw new ArgumentException("Invalid answer")
        };

    internal static double GetVolunteering(A4 answer) => answer.Answer switch
    {
        A4.AtLeastOncePerMonth => 1,
        A4.LessThanOncePerMonth => 0.82,
        A4.NoCannot => 0.7,
        A4.NoPreferNot => 0.25,
        _ => throw new ArgumentException("Invalid answer")
    };

    internal static double GetJobSearch(A5 answer) => answer.Answer switch
    {
        A5.Yes => 1,
        A5.No => 0.01,
        _ => throw new ArgumentException("Invalid answer")
    };
}
