namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.HealthAndAdditiction;

public class E6() : MultipleChoiceQuestion("Do you have a problem, receive, or require help with any of the following? (tick all that apply)",
    "Illegal drugs include substances such as cannabis, spice, heroin, and cocaine. Prescription drugs include substances such as methadone, painkillers, antidepressants, benzos and sleeping pills.",
    [
        Alcohol,
        Gambling,
        IllegalDrugsOrSubstances,
        SexOrPornography,
        PrescriptionDrugs,
        Food,
        SomethingElse,
        None
    ])
{
    public const string Alcohol = "Alcohol";
    public const string Gambling = "Gambling";
    public const string IllegalDrugsOrSubstances = "Illegal drugs / substances";
    public const string SexOrPornography = "Sex or pornography";
    public const string PrescriptionDrugs = "Prescription drugs";
    public const string Food = "Food";
    public const string SomethingElse = "Something else";
    public const string None = "None";
}