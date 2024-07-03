namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Education;

public partial class EducationPathway
{
    protected override IEnumerable<double> GetPercentiles(int age, AssessmentLocation location, Sex sex)
    {
        double[] percentiles = [
            GetHighestLevelOfQualification(),
            GetFinishSchool(),
            GetNeurodiversity(),
            GetLearningDifficulties(),
            GetCreativeActivity(),
        ];
        return percentiles;
    }
    private double GetCreativeActivity()
    {
        double weight = 0.11;

        var count = D5.Answers?.Except([D5.NoneOftheGivenOptions]).Count();
        double percentile = count switch
        {
            0 => 0.28,
            1 => 0.39,
            2 => 0.52,
            _ => 1
        };

        return percentile.PowerRound(weight);
    }
    private double GetLearningDifficulties()
    {
        double weight = 0.16;

        var count = D4.Answers?.Except([D4.NoneOfThese]).Count();
        double percentile = count switch
        {
            0 => 1,
            1 => 0.1,
            2 => 0.05,
            _ => 0.01
        };

        return percentile.PowerRound(weight);
    }
    private double GetNeurodiversity()
    {
        double weight = 0.08;

        // join on the options
        var query = from a in D3.Answers
            join s in NeurodiveristyMappings on a equals s.key
            select s.percentile;

        var percentile = query.Min();

        return percentile.PowerRound(weight);

    }
    private double GetFinishSchool()
    {
        double weight = 0.15;

        double percentile = D2.Answer switch
        {
            D2.YesFinishedSchool => 1,
            D2.LeftBefore16 => 0.01,
            D2.LeftBefore11 => 0.001,
            _ => throw new ApplicationException("Unknown answer")
        };
        
        return percentile.PowerRound(weight);
    }
    private double GetHighestLevelOfQualification()
    {
        double weight = 0.5;

        double percentile = D1.Answer switch
        {
            D1.DegreeOrHigher => 1,
            D1.TwoPlusALevelsOrSimilar => 0.65,
            D1.FivePlusGcsesOrSimilar => 0.42,
            D1.NvqLevelOneOrSimilar => 0.29,
            D1.NoQualificationsOrEntryLevel => 0.19,
            _ => throw new ApplicationException("Unknown answer")
        };
        
        return percentile.PowerRound(weight);
    }
    
    private static readonly List<(string key, double percentile)> NeurodiveristyMappings =
    [
        (D3.AutismOrASD, 0.01),
        (D3.AdhdOrAdd, 0.03),
        (D3.Epilepsy, 0.01),
        (D3.Synaesthesia, 0.03),
        (D3.TouretteSyndrome, 0.01),
        (D3.IntellectualDisability, 0.01),
        (D3.Dyslexia, 0.10),
        (D3.Dyspraxia, 0.04),
        (D3.Dyscalculia, 0.06),
        (D3.Dysgraphia, 0.10),
        (D3.OtherLearningDisability, 0.01),
        (D3.NoneOftheGivenOptions, 1)
    ];
    
}
