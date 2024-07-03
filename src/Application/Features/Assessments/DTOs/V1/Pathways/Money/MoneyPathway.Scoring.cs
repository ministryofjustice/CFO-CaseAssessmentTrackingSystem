namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Money;

public partial class MoneyPathway
{
    protected override IEnumerable<double> GetPercentiles(int age, AssessmentLocation location, Sex sex)
    {
        double[] percentiles =
        [
            GetCopingFinancially(),
            GetPayingBills(),
            GetProblemDebt(),
            GetBanking(),
            GetFoodSecurity(),
        ];
        return percentiles;
    }
    private double GetFoodSecurity()
    {
        double weight = 0.12;
        double percentile = C5.Answer switch
        {
            C9.Never => 1,
            C9.OverYearAgo => 1,
            C9.WithinTheLastYear => 0.03,
            C9.WithinTheLast30Days => 0.01,
            _ => throw new ApplicationException("Unknown answer")
        };
        return percentile.PowerRound(weight);
    }
    private double GetBanking()
    {
        double weight = 0.12;
        double percentile = C5.Answer switch
        {
            C5.Yes => 1,
            C5.No => 0.02,
            _ => throw new ApplicationException("Unknown answer")
        };
        return percentile.PowerRound(weight);
    }
    private double GetProblemDebt()
    {
        double weight = 0.15;
        double percentile = C2.Answer switch
        {
            C3.Yes => 1,
            C3.No => 0.29,
            _ => throw new ApplicationException("Unknown answer")
        };
        return percentile.PowerRound(weight);
    }
    private double GetPayingBills()
    {
        double weight = 0.11;
        double percentile = C2.Answer switch
        {
            C2.Yes => 1,
            C2.No => 0.16,
            _ => throw new ApplicationException("Unknown answer")
        };
        return percentile.PowerRound(weight);
    }
    private double GetCopingFinancially()
    {
        double weight = 0.5;
        double percentile = C1.Answer switch
        {
            C1.LivingComfortably => 1,
            C1.DoingAlright => 0.7,
            C1.JustAboutGettingBy => 0.26,
            C1.FindingQuiteDifficult => 0.07,
            C1.FindingVeryDifficult => 0.02,
            _ => throw new ApplicationException("Unknown answer")
        };
        return percentile.PowerRound(weight);
    }
}
