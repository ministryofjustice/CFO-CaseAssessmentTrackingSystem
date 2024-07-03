namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.HealthAndAdditiction;

public class E7() : MultipleChoiceQuestion("Have you previously had a problem or received help with any of the following? (tick all that apply)",
    "See above for guidance.",
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