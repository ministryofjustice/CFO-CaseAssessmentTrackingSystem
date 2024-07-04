namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;

public partial class RelationshipsPathway
{
    protected override IEnumerable<double> GetPercentiles(int age, AssessmentLocation location, Sex sex)
    {
        double[] percentiles = [
            GetUnhappyRelationships(),
            GetLoneliness(),
            GetSocial(),
            GetTrust(),
            GetCaring(),
        ];
        return percentiles;
    }

    private double GetCaring()
    {
        double weight = 0.07;
        double percentile = F8.Answer switch 
        {
            F8.No => 1.0,
            F8.UnderTenHoursPerWeek => 0.09,
            F8.Between10And34HoursPerWeek => 0.06,
            F8.Between35And49HoursPerWeek => 0.04,
            F8.FiftyPlusHoursPerWeek => 0.03,
            _ => throw new ApplicationException("Unknown answer")
        };
        return percentile.PowerRound(weight);
    }

    private double GetTrust()
    {
        const double weight = 0.5;
        double percentile = F6.Answer switch 
        {
            F6.Completely => 1.0,
            F6.Mostly => 0.94,
            F6.Somewhat => 0.39,
            F6.ALittle => 0.13,
            F6.NotAtAll => 0.04,
            _ => throw new ApplicationException("Unknown answer"),
        };
        return percentile.PowerRound(weight);        
    }

    private double GetSocial()
    {
        const double weight = 0.06;
        double percentile = F5.Answer switch 
        {
            F5.AtleastOncePerWeek => 1.0,
            F5.LessThanOncePerWeek => 0.26,
            F5.RarelyOrNever => 0.26,
            _ => throw new ApplicationException("Unknown answer")
        };
        return percentile.CompareTo(weight);
    }

    private double GetLoneliness()
    {
        const double weight = 0.19;
        double percentile = F2.Answer switch
        {
            F2.Never => 1.0,
            F2.HardlyEver => 0.79,
            F2.Occasionally => 0.47,
            F2.Sometimes => 0.25,
            F2.OftenOrAlways => 0.06,
            _ => throw new ApplicationException("Unknown answer")
        };
        return percentile.PowerRound(weight);
    }

    private double GetUnhappyRelationships()
    {
        const double weight = 0.18;
        double percentile = F1.Answer switch {
            F1.ExtremelyHappy => 1.0,
            F1.VeryHappy => 0.69,
            F1.Happy => 0.40,
            F1.FairlyUnhappy => 0.12,
            F1.ExtremelyUnhappy => 0.02,
            _ => throw new ApplicationException("Unknown answer")
        };
        return percentile.PowerRound(weight);
    }
}