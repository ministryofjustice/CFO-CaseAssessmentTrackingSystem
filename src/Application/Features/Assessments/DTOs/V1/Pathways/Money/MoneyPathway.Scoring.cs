namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Money;

public partial class MoneyPathway
{
    internal override IEnumerable<double> GetPercentiles(int age, AssessmentLocation location, Sex sex)
    {
        double[] percentiles =
        [
            GetCopingFinancially(C1).PowerRound(0.5),
            GetPayingBills(C2).PowerRound(0.11),
            GetProblemDebt(C3).PowerRound(0.15),
            GetBanking(C5).PowerRound(0.12),
            GetFoodSecurity(C9).PowerRound(0.12)
        ];
        return percentiles;
    }
    internal static double GetFoodSecurity(C9 answer)
        => answer.Answer switch
        {
            C9.Never => 1,
            C9.OverYearAgo => 1,
            C9.WithinTheLastYear => 0.03,
            C9.WithinTheLast30Days => 0.01,
            _ => throw new ApplicationException("Unknown answer")
        };

    internal static double GetBanking(C5 answer)
        => answer.Answer switch
        {
            C5.Yes => 1,
            C5.No => 0.02,
            _ => throw new ApplicationException("Unknown answer")
        };
    internal static double GetProblemDebt(C3 answer)
        => answer.Answer switch
        {
            C3.Yes => 1,
            C3.No => 0.29,
            _ => throw new ApplicationException("Unknown answer")
        };

    internal static double GetPayingBills(C2 answer)
        => answer.Answer switch
        {
            C2.Yes => 1,
            C2.No => 0.16,
            _ => throw new ApplicationException("Unknown answer")
        };
    internal static double GetCopingFinancially(C1 answer)
        => answer.Answer switch
        {
            C1.LivingComfortably => 1,
            C1.DoingAlright => 0.7,
            C1.JustAboutGettingBy => 0.26,
            C1.FindingQuiteDifficult => 0.07,
            C1.FindingVeryDifficult => 0.02,
            _ => throw new ApplicationException("Unknown answer")
        };
}
