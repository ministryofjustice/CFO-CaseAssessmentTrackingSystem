namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.HealthAndAdditiction;

public partial class HealthAndAddictionPathway
{
    protected override IEnumerable<double> GetPercentiles(int age, AssessmentLocation location, Sex sex)
    {
        double[] percentiles = [
                GetGeneralHealth(age),
                GetDisablilty(),
                GetRegistered(),
                GetExercise(),
                GetAlcohol(),
                GetDrugs(),
                GetGambling(),
                GetSmokingAndVaping(),
            ];
        return percentiles;
    }
    private double GetSmokingAndVaping()
    {
        const double weight = 0.1;

        double smoker = E8.Answer switch
        {
            E8.NeverSmoked => 1,
            E8.IUsedTo => 0.39,
            E8.Yes => 0.13,
            _ => throw new ApplicationException("Unknown answer")
        };
        
        double vaper = E9.Answer switch
        {
            E9.NeverVaped => 1,
            E9.IUsedTo => 0.39,
            E9.Yes => 0.13,
            _ => throw new ApplicationException("Unknown answer")
        };

        return Math.Min(smoker, vaper).PowerRound(weight);

    }
    private double GetAlcohol()
    {
        const double weight = 0.05;
        double percentile = E6.Answers!.Any(a => a == E6.Alcohol) ? 0.01 : 1;
        return percentile.PowerRound(weight);
    }
    
    private double GetGambling()
    {
        const double weight = 0.01;
        double percentile = E6.Answers!.Any(a => a == E6.Gambling) ? 0.01 : 1;
        return percentile.PowerRound(weight);
    }

    
    private double GetDrugs()
    {
        const double weight = 0.11;
        double percentile = E6.Answers!.Any(a => a == E6.IllegalDrugsOrSubstances) ? 0.01 : 1;
        return percentile.PowerRound(weight);
    }
    
    private double GetExercise()
    {
        const double weight = 0.09;
        double percentile = E5.Answer switch
        {
            E5.Over2AndHalfHoursPerWeek => 1,
            E5.ThirtyMinutesTo2AndHalfHoursPerWeek => 0.37,
            E5.LessThan30MinutesPerWeek => 0.26,
            _ => throw new ApplicationException("Unknown answer")
        };
        return percentile.PowerRound(weight);
    }
    private double GetRegistered()
    {
        const double weight = 0.11;
        double percentile = E4.Answer switch
        {
            E4.RegisteredWithGPAndDentist => 1,
            E4.RegisteredWithGPOnly => 0.22,
            E4.NotRegisteredWithGPOrDentist => 0.001,
            E4.RegisteredWithDentistOnly => 0.001,
            _ => throw new ApplicationException("Unknown answer")
        };
        return percentile.PowerRound(weight);
    }
    private double GetDisablilty()
    {
        const double weight = 0.03;
        double percentile = E2.Answer switch
        {
            E2.No => 1,
            E2.LittleImpairment => 0.2,
            E2.LotOfImpairment => 0.06,
            _ => throw new ApplicationException("Unknown answer")
        };
        return percentile.PowerRound(weight);
    }
    private double GetGeneralHealth(int age)
    {
        const double weight = 0.5;

        double percentile = (age, E1.Answer) switch
        {
            (< 20, E1.VeryGood) => 1,
            (< 20, E1.Good) => 0.3,
            (< 20, E1.Fair) => 0.06,
            (< 20, E1.Bad) => 0.01,
            (< 20, E1.VeryBad) => 0.001,
            (< 24, E1.VeryGood) => 1,
            (< 24, E1.Good) => 0.39,
            (< 24, E1.Fair) => 0.08,
            (< 24, E1.Bad) => 0.02,
            (< 24, E1.VeryBad) => 0.001,
            (< 30, E1.VeryGood) => 1,
            (< 30, E1.Good) => 0.42,
            (< 30, E1.Fair) => 0.09,
            (< 30, E1.Bad) => 0.02,
            (< 30, E1.VeryBad) => 0.001,
            (< 35, E1.VeryGood) => 1,
            (< 35, E1.Good) => 0.45,
            (< 35, E1.Fair) => 0.10,
            (< 35, E1.Bad) => 0.02,
            (< 35, E1.VeryBad) => 0.001,
            (< 40, E1.VeryGood) => 1,
            (< 40, E1.Good) => 0.50,
            (< 40, E1.Fair) => 0.11,
            (< 40, E1.Bad) => 0.03,
            (< 40, E1.VeryBad) => 0.01,
            (< 45, E1.VeryGood) => 1,
            (< 45, E1.Good) => 0.54,
            (< 45, E1.Fair) => 0.14,
            (< 45, E1.Bad) => 0.04,
            (< 45, E1.VeryBad) => 0.01,
            (< 50, E1.VeryGood) => 1,
            (< 50, E1.Good) => 0.58,
            (< 50, E1.Fair) => 0.17,
            (< 50, E1.Bad) => 0.05,
            (< 50, E1.VeryBad) => 0.01,
            (< 55, E1.VeryGood) => 1,
            (< 55, E1.Good) => 0.62,
            (< 55, E1.Fair) => 0.21,
            (< 55, E1.Bad) => 0.06,
            (< 55, E1.VeryBad) => 0.02,
            (< 60, E1.VeryGood) => 1,
            (< 60, E1.Good) => 0.66,
            (< 60, E1.Fair) => 0.24,
            (< 60, E1.Bad) => 0.08,
            (< 60, E1.VeryBad) => 0.02,
            (< 65, E1.VeryGood) => 1,
            (< 65, E1.Good) => 0.70,
            (< 65, E1.Fair) => 0.29,
            (< 65, E1.Bad) => 0.10,
            (< 65, E1.VeryBad) => 0.02,
            (_, E1.VeryGood) => 1,
            (_, E1.Good) => 0.75,
            (_, E1.Fair) => 0.32,
            (_, E1.Bad) => 0.10,
            (_, E1.VeryBad) => 0.02,
            _ => throw new ApplicationException("Unknown answer")
        };

        return percentile.PowerRound(weight);

    }

}
