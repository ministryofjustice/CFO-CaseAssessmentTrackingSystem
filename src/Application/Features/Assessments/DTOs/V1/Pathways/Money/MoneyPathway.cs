namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Money;

public class MoneyPathway : PathwayBase
{

    public MoneyPathway()
    {
        Questions = 
        [
            C1 = new C1(),
            C2 = new C2(),
            C3 = new C3(),
            C4 = new C4(),
            C5 = new C5(),
            C6 = new C6(),
            C7 = new C7(),
            C8 = new C8(),
            C9 = new C9(),
        ];
    }

    public C1 C1 { get; private set; }
    public C2 C2 { get; private set; }
    
    public C3 C3 { get; private set; }
    
    public C4 C4 { get; private set; }
    
    public C5 C5 { get; private set; }
    
    public C6 C6 { get; private set; }
    
    public C7 C7 { get; private set; }
    public C8 C8 { get; private set; }
    
    public C9 C9 { get; private set; }
    
    
    public override string Title => "Money";
    public override double Constant => 0.76845;
    public override string Icon => CatsIcons.Money;
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