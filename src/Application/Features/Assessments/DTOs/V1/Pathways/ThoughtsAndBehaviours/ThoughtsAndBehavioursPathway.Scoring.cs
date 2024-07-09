using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;

namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.ThoughtsAndBehaviours;

public partial class ThoughtsAndBehavioursPathway
{
    internal override IEnumerable<double> GetPercentiles(int age, AssessmentLocation location, Sex sex)
    {
        double[] percentiles =
        [
            GetResilience(G1).PowerRound(0.05),
            GetImpulsivity(age, G2).PowerRound(0.5),
            GetMotivation(age, G3).PowerRound(0.02),
            GetHope(G4).PowerRound(0.08),
            GetSelfEsteem(G5).PowerRound(0.05),
            GetAdaptability(G6).PowerRound(0.07),
            GetEmotionalPerception(G7).PowerRound(0.09),
            GetEmotionalRegulation(age, G8).PowerRound(0.07),
            GetSocialSkills(G9).PowerRound(0.07)
        ];
        return percentiles;
    }

    internal static double GetSocialSkills(G9 answer)
        => answer.Answer switch
        {
            G9.StronglyAgree => 1,
            G9.Agree => 0.71,
            G9.Neither => 0.25,
            G9.Disagree => 0.15,
            G9.StronglyDisagree => 0.04,

            _ => throw new ApplicationException("Unknown answer")
        };

    internal static double GetEmotionalRegulation(int age, G8 answer)
        => (answer.Answer, age) switch
        {
            (G8.Agree, < 25) => 0.64,
            (G8.Agree, < 35) => 0.54,
            (G8.Agree, < 45) => 0.44,
            (G8.Agree, < 55) => 0.34,
            (G8.Agree, < 65) => 0.25,
            (G8.Agree, _) => 0.15,

            (G8.Disagree, < 25) => 0.99,
            (G8.Disagree, < 35) => 0.96,
            (G8.Disagree, < 45) => 0.92,
            (G8.Disagree, < 55) => 0.89,
            (G8.Disagree, < 65) => 0.85,
            (G8.Disagree, _) => 0.82,

            (G8.Neither, < 25) => 0.76,
            (G8.Neither, < 35) => 0.69,
            (G8.Neither, < 45) => 0.62,
            (G8.Neither, < 55) => 0.55,
            (G8.Neither, < 65) => 0.48,
            (G8.Neither, _) => 0.41,

            (G8.StronglyAgree, _) => 0.07,

            (G8.StronglyDisagree, _) => 1,

            _ => throw new ApplicationException("Unknown answer")
        };

    internal static double GetEmotionalPerception(G7 answer)
        => answer.Answer switch
        {
            G7.Agree => 0.15,
            G7.Disagree => 0.7,
            G7.Neither => 0.22,
            G7.StronglyAgree => 0.02,
            G7.StronglyDisagree => 1,
            _ => throw new ApplicationException("Unknown answer")
        };

    internal static double GetAdaptability(G6 answer)
        => answer.Answer switch
        {
            G6.Agree => 0.85,
            G6.Disagree => 0.18,
            G6.Neither => 0.35,
            G6.StronglyAgree => 1,
            G6.StronglyDisagree => 0.01,
            _ => throw new ApplicationException("Unknown answer")
        };

    internal static double GetSelfEsteem(G5 answer)
        => answer.Answer switch
        {
            G5.Agree => 0.88,
            G5.Disagree => 0.18,
            G5.Neither => 0.37,
            G5.StronglyAgree => 1,
            G5.StronglyDisagree => 0.02,
            _ => throw new ApplicationException("Unknown answer")
        };

    internal static double GetHope(G4 answer)
        => answer.Answer switch
        {
            G4.Agree => 0.09,
            G4.Disagree => 0.77,
            G4.Neither => 0.25,
            G4.StronglyAgree => 0.02,
            G4.StronglyDisagree => 1,
            _ => throw new ApplicationException("Unknown answer")
        };

    internal static double GetMotivation(int age, G3 answer)
        => (answer.Answer, age) switch
        {
            (G3.Agree, < 25) => 0.76,
            (G3.Agree, < 35) => 0.79,
            (G3.Agree, < 45) => 0.83,
            (G3.Agree, < 55) => 0.87,
            (G3.Agree, < 65) => 0.9,
            (G3.Agree, _) => 0.94,

            (G3.Disagree, _) => 0.06,

            (G3.Neither, < 25) => 0.18,
            (G3.Neither, < 35) => 0.23,
            (G3.Neither, < 45) => 0.28,
            (G3.Neither, < 55) => 0.34,
            (G3.Neither, < 65) => 0.39,
            (G3.Neither, _) => 0.44,

            (G3.StronglyAgree, _) => 1,
            (G3.StronglyDisagree, _) => 0.01,

            _ => throw new ApplicationException("Unknown answer")
        };
    internal static double GetImpulsivity(int age, G2 answer)
        => (answer.Answer, age) switch
        {
            (G2.Agree, < 25) => 0.09,
            (G2.Agree, < 35) => 0.09,
            (G2.Agree, < 45) => 0.09,
            (G2.Agree, < 55) => 0.09,
            (G2.Agree, < 65) => 0.09,
            (G2.Agree, _) => 0.09,
            (G2.Disagree, < 25) => 0.58,
            (G2.Disagree, < 35) => 0.66,
            (G2.Disagree, < 45) => 0.73,
            (G2.Disagree, < 55) => 0.80,
            (G2.Disagree, < 65) => 0.88,
            (G2.Disagree, _) => 0.95,
            (G2.Neither, < 25) => 0.13,
            (G2.Neither, < 35) => 0.17,
            (G2.Neither, < 45) => 0.20,
            (G2.Neither, < 55) => 0.23,
            (G2.Neither, < 65) => 0.27,
            (G2.Neither, _) => 0.30,
            (G2.StronglyAgree, _) => 0.01,
            (G2.StronglyDisagree, _) => 1,
            _ => throw new ApplicationException("Unknown answer")
        };
    internal static double GetResilience(G1 answer)
        => answer.Answer switch
        {
            G1.Agree => 0.81,
            G1.Disagree => 0.13,
            G1.Neither => 0.26,
            G1.StronglyAgree => 1,
            G1.StronglyDisagree => 0.02,
            _ => throw new ApplicationException("Unknown answer")
        };
}
