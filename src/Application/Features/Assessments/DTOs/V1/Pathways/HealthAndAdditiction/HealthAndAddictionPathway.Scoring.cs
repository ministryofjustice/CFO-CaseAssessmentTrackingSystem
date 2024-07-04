namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.HealthAndAdditiction;

public partial class HealthAndAddictionPathway
{
    internal override IEnumerable<double> GetPercentiles(int age, AssessmentLocation location, Sex sex)
    {
        double[] percentiles =
        [
            GetGeneralHealth(age, E1).PowerRound(0.5),
            GetDisablilty(E2).PowerRound(0.03),
            GetRegistered(E4).PowerRound(0.11),
            GetExercise(E5).PowerRound(0.09),
            GetAlcohol(E6).PowerRound(0.05),
            GetDrugs(E6).PowerRound(0.11),
            GetGambling(E6).PowerRound(0.01),
            GetSmokingAndVaping(E8, E9).PowerRound(0.1)
        ];
        return percentiles;
    }

    internal static double GetSmokingAndVaping(E8 smokingAnswer, E9 vapingAnswer)
    {
        var smoker = smokingAnswer.Answer switch
        {
            E8.NeverSmoked => 1,
            E8.IUsedTo => 0.39,
            E8.Yes => 0.13,
            _ => throw new ApplicationException("Unknown answer")
        };

        var vaper = vapingAnswer.Answer switch
        {
            E9.NeverVaped => 1,
            E9.IUsedTo => 0.39,
            E9.Yes => 0.13,
            _ => throw new ApplicationException("Unknown answer")
        };

        return Math.Min(smoker, vaper);
    }
    internal static double GetAlcohol(E6 answer)
        => answer.Answers!.Any(a => a == E6.Alcohol) ? 0.01 : 1;

    internal static double GetGambling(E6 answer)
        => answer.Answers!.Any(a => a == E6.Gambling) ? 0.01 : 1;

    internal static double GetDrugs(E6 answer)
        => answer.Answers!.Any(a => a == E6.IllegalDrugsOrSubstances) ? 0.01 : 1;

    internal static double GetExercise(E5 answer)
        => answer.Answer switch
        {
            E5.Over2AndHalfHoursPerWeek => 1,
            E5.ThirtyMinutesTo2AndHalfHoursPerWeek => 0.37,
            E5.LessThan30MinutesPerWeek => 0.26,
            _ => throw new ApplicationException("Unknown answer")
        };
    
    internal static double GetRegistered(E4 answer) 
        => answer.Answer switch
    {
        E4.RegisteredWithGPAndDentist => 1,
        E4.RegisteredWithGPOnly => 0.22,
        E4.NotRegisteredWithGPOrDentist => 0.001,
        E4.RegisteredWithDentistOnly => 0.001,
        _ => throw new ApplicationException("Unknown answer")
    };
    internal static double GetDisablilty(E2 answer) => answer.Answer switch
    {
        E2.No => 1,
        E2.LittleImpairment => 0.2,
        E2.LotOfImpairment => 0.06,
        _ => throw new ApplicationException("Unknown answer")
    };
    internal static double GetGeneralHealth(int age, E1 answer)
        => (answer.Answer, age) switch
        {
            (E1.Bad, < 20) => 0.01,
            (E1.Bad, < 40) => 0.02,
            (E1.Bad, < 45) => 0.04,
            (E1.Bad, < 50) => 0.05,
            (E1.Bad, < 55) => 0.06,
            (E1.Bad, < 60) => 0.08,
            (E1.Bad, _) => 0.10,

            (E1.Fair, < 20) => 0.06,
            (E1.Fair, < 24) => 0.08,
            (E1.Fair, < 30) => 0.09,
            (E1.Fair, < 35) => 0.10,
            (E1.Fair, < 40) => 0.11,
            (E1.Fair, < 45) => 0.14,
            (E1.Fair, < 50) => 0.17,
            (E1.Fair, < 55) => 0.21,
            (E1.Fair, < 60) => 0.24,
            (E1.Fair, < 65) => 0.29,
            (E1.Fair, _) => 0.32,

            (E1.Good, < 20) => 0.3,
            (E1.Good, < 24) => 0.39,
            (E1.Good, < 30) => 0.42,
            (E1.Good, < 35) => 0.45,
            (E1.Good, < 40) => 0.50,
            (E1.Good, < 45) => 0.54,
            (E1.Good, < 50) => 0.58,
            (E1.Good, < 55) => 0.62,
            (E1.Good, < 60) => 0.66,
            (E1.Good, < 65) => 0.70,
            (E1.Good, _) => 0.75,

            (E1.VeryBad, < 40) => 0.001,
            (E1.VeryBad, _) => 0.02,

            (E1.VeryGood, _) => 1,

            _ => throw new ApplicationException("Unknown answer")
        };
}
