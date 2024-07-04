namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.WellbeingAndMentalHealth;

public class H13() : MultipleChoiceQuestion("Do you live with any of the following mental health conditions? (tick all that apply)", "It is ok to tick if you have not been formally diagnosed by a doctor yet if you feel you have it.", 
    [
        AnxietyDisorders,
        BipolarDisorder,
        BorderlinePersonalityDisorder,
        Depression,
        EatingDisorders,
        PanicDisorder,
        ObsessiveCompulsiveDisorder,
        Psychosis,
        PersonalityDisorders,
        Schizophrenia,
        PostTraumaticStressDisorder,
        SelfHarm,
        Other,
        NoneOfThese])
{
    public const string AnxietyDisorders = "Anxiety disorders";
    public const string BipolarDisorder = "Bipolar Disorder";
    public const string BorderlinePersonalityDisorder = "Borderline Personality Disorder";
    public const string Depression = "Depression";
    public const string EatingDisorders = "Eating Disorders";
    public const string PanicDisorder = "Panic Disorder";
    public const string ObsessiveCompulsiveDisorder = "Obsessive Compulsive Disorder";
    public const string Psychosis = "Psychosis";
    public const string PersonalityDisorders = "Personality Disorders";
    public const string Schizophrenia = "Schizophrenia";
    public const string PostTraumaticStressDisorder = "Post-Traumatic Stress Disorder";
    public const string SelfHarm = "Self-harm";
    public const string Other = "Other";
    public const string NoneOfThese = "None of these";
}
