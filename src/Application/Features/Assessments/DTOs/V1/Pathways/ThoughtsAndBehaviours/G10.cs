namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;

public class G10() : MultipleChoiceQuestion("Which of the following is/are the most important to you?",
    "You can select more than one option if you want to, but you must select at least one.",
    [
        YourOffending,
        LearningAndEducation,
        WellbeingAndMentalHealth,
        HousingSituation,
        PhysicalHealth,
        FinancialSituation,
        Addiction,
        Work,
        ThoughtsAndBehaviour,
        SomethingElse,
        Relationships,
        Nothing
    ])
{
    public const string YourOffending = "Your offending";
    public const string LearningAndEducation = "Learning & education";
    public const string WellbeingAndMentalHealth = "Your wellbeing & mental health";
    public const string HousingSituation = "Your housing situation";
    public const string PhysicalHealth = "Your physical health";
    public const string FinancialSituation = "Your financial situation";
    public const string Addiction = "An addiction";
    public const string Work = "Getting into or keeping work";
    public const string ThoughtsAndBehaviour = "Your thoughts and behaviour";
    public const string SomethingElse = "Something else";
    public const string Relationships = "Your relationships, family, friends";
    public const string Nothing = "Nothing";
}
