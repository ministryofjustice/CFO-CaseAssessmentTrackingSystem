namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Education;
public class D3() : MultipleChoiceQuestion("Have you been diagnosed with or feel you may have any of the following? (tick all that apply)",
    "It is ok to tick if you have not been formally diagnosed by a doctor yet if you feel you have it.",
    [
        AutismOrASD,
        AdhdOrAdd,
        Epilepsy,
        Synaesthesia,
        TouretteSyndrome,
        IntellectualDisability,
        Dyslexia,
        Dyspraxia,
        Dyscalculia,
        Dysgraphia,
        OtherLearningDisability,
        NoneOftheGivenOptions
    ])
{
    public const string AutismOrASD = "Autism / ASD";
    public const string AdhdOrAdd= "ADHD / ADD";
    public const string Epilepsy = "Epilepsy";
    public const string Synaesthesia = "Synaesthesia";
    public const string TouretteSyndrome = "Tourette syndrome";
    public const string IntellectualDisability = "An intellectual disability";
    public const string Dyslexia = "Dyslexia";
    public const string Dyspraxia = "Dyspraxia";
    public const string Dyscalculia = "Dyscalculia";
    public const string Dysgraphia = "Dysgraphia";
    public const string OtherLearningDisability = "Other learning disability";
    public const string NoneOftheGivenOptions = "None of these";

}
