namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;

public class H11() : MultipleChoiceQuestion("Do you live with any of the following mental health conditions? (tick all that apply)",
    "It is ok to tick if you have not been formally diagnosed by a doctor yet if you feel you have it",
    [
        AnxietyDisorders,
        BipolarDisorder,
        BorderlinePersonalityDisorder,
        Depression,
        EatingDisorders,
        PanicDisorder,
        OCD,
        Psychosis,
        PersonalityDisorders,
        Schizophrenia,
        PTSD,
        SelfHarm,
        Other,
        NoneOfThese,
    ])
{
    public const string AnxietyDisorders = "Anxiety disorders";
    public const string BipolarDisorder = "Bipolar Disorder";
    public const string BorderlinePersonalityDisorder = "Borderline Personality Disorder";
    public const string Depression = "Depression";
    public const string EatingDisorders = "Eating Disorders";
    public const string PanicDisorder = "Panic Disorder";
    public const string OCD = "Obsessive Compulsive Disorder";
    public const string Psychosis = "Psychosis";
    public const string PersonalityDisorders = "Personality Disorders";
    public const string Schizophrenia = "Schizophrenia";
    public const string PTSD = "Post-Traumatic Stress Disorder";
    public const string SelfHarm = "Self-harm";
    public const string Other = "Other";
    public const string NoneOfThese = "None of these";
    
}
