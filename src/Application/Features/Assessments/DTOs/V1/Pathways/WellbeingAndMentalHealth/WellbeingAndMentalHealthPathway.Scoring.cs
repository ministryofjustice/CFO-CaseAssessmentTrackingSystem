using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;

namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.WellbeingAndMentalHealth;

public partial class WellbeingAndMentalHealthPathway
{
    internal override IEnumerable<double> GetPercentiles(int age, AssessmentLocation location, Sex sex)
        =>
        [
            GetLifeSatisfaction(H11).PowerRound(0.5),
            GetMentalDistress(this).PowerRound(0.35),
            GetStressed(H12).PowerRound(0.15)
        ];
    internal static double GetStressed(H12 answer)
        => answer.Answer switch
        {
            H12.RarelyOrNever => 1,
            H12.FewTimesAMonth => 0.82,
            H12.OneToTwoTimesPerWeek => 0.53,
            H12.EveryDay => 0.26,
            _ => throw new ApplicationException("Unknown answer")
        };

    internal static double GetMentalDistress(WellbeingAndMentalHealthPathway pathway)
    {
        /*
         This requires a two-step process.
        1. Score each of the 10 questions H1 to H10 based on the table below and sum to get a total score. Total score will be between 0 and 40.
        2. Percentile is based on the total score.
        */

        var sum = Score(pathway.H1.Answer!) +
                  ReverseScore(pathway.H2.Answer!) + 
                  Score(pathway.H3.Answer!) + 
                  Score(pathway.H4.Answer!) + 
                  Score(pathway.H5.Answer!) + 
                  Score(pathway.H6.Answer!) + 
                  Score(pathway.H7.Answer!) + 
                  Score(pathway.H8.Answer!) + 
                  Score(pathway.H9.Answer!) + 
                  ReverseScore(pathway.H10.Answer!);

        return sum switch
        {
            0 or 1 => 1.00,
            2 or 3 => 0.73,
            4 or 5 => 0.45,
            6 or 7 => 0.30,
            8 or 9 => 0.19,
            10 or 11 => 0.12,
            12 or 13 => 0.06,
            14 or 15 => 0.05,
            16 or 17 => 0.03,
            18 or 19 => 0.02,
            20 or 21 => 0.01,
            >= 22 and <= 40 => 0.001,
            _ => throw new ApplicationException("Invalid sum")
        };

        double Score(string answer) =>
            answer switch
            {
                HealthCoreQuestion.NotAtAll => 0,
                HealthCoreQuestion.OnlyOccasionally => 1,
                HealthCoreQuestion.Sometimes => 2,
                HealthCoreQuestion.Often => 3,
                HealthCoreQuestion.MostOrAllOfTheTime => 4,
                _ => throw new ApplicationException("Unknown answer")
            };

        double ReverseScore(string answer) =>
            answer switch
            {
                HealthCoreQuestion.NotAtAll => 4,
                HealthCoreQuestion.OnlyOccasionally => 3,
                HealthCoreQuestion.Sometimes => 2,
                HealthCoreQuestion.Often => 1,
                HealthCoreQuestion.MostOrAllOfTheTime => 0,
                _ => throw new ApplicationException("Unknown answer")
            };
    }
    
    internal static double GetLifeSatisfaction(H11 answer)
        => answer.Answer switch
        {
            H11.Completely => 1,
            H11.Mostly => 0.75,
            H11.Fairly => 0.21,
            H11.MostlyNot => 0.05,
            H11.NotAtAll => 0.025,
            _ => throw new ApplicationException("Unknown answer")
        };

}
