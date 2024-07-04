namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;

public partial class RelationshipsPathway
{
    internal override IEnumerable<double> GetPercentiles(int age, AssessmentLocation location, Sex sex)
    {
        double[] percentiles = [
            GetUnhappyRelationships(F1).PowerRound(0.18),
            GetLoneliness(F2).PowerRound(0.19),
            GetSocial(F5).PowerRound(0.06),
            GetTrust(F6).PowerRound(0.5),
            GetCaring(F8).PowerRound(0.07),
        ];
        return percentiles;
    }

    internal static double GetCaring(F8 answer) 
        => answer.Answer switch 
    {
        F8.No => 1.0,
        F8.UnderTenHoursPerWeek => 0.09,
        F8.Between10And34HoursPerWeek => 0.06,
        F8.Between35And49HoursPerWeek => 0.04,
        F8.FiftyPlusHoursPerWeek => 0.03,
        _ => throw new ApplicationException("Unknown answer")
    };

    internal static double GetTrust(F6 answer) 
        => answer.Answer switch 
    {
        F6.Completely => 1.0,
        F6.Mostly => 0.94,
        F6.Somewhat => 0.39,
        F6.ALittle => 0.13,
        F6.NotAtAll => 0.04,
        _ => throw new ApplicationException("Unknown answer"),
    };

    internal static double GetSocial(F5 answer) 
        => answer.Answer switch 
    {
        F5.AtleastOncePerWeek => 1.0,
        F5.LessThanOncePerWeek => 0.26,
        F5.RarelyOrNever => 0.26,
        _ => throw new ApplicationException("Unknown answer")
    };

    internal static double GetLoneliness(F2 answer) 
        => answer.Answer switch
    {
        F2.Never => 1.0,
        F2.HardlyEver => 0.79,
        F2.Occasionally => 0.47,
        F2.Sometimes => 0.25,
        F2.OftenOrAlways => 0.06,
        _ => throw new ApplicationException("Unknown answer")
    };

    internal static double GetUnhappyRelationships(F1 answer, double weight = 0.18)
    {
        double percentile = answer.Answer switch {
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