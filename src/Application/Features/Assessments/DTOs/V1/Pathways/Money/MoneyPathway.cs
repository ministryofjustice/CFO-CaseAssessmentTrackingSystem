namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Money;

public class MoneyPathway : PathwayBase
{
    public override string Title => "Money";
    public override double Constant => 0.76845;
    public override string Icon => CatsIcons.Money;
    protected override IEnumerable<double> GetScores()
    {
        yield return 0;
    }
}