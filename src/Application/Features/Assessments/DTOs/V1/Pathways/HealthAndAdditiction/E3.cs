namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.HealthAndAdditiction;

public class E3() : SingleChoiceQuestion("Are you currently taking any regular medication or undergoing treatment for a physical or mental health condition?",
    [
        YesMentalHealthMedication,
        NoMentalHealthMedication,
        NotSureMentalHealthMedication
    ])
{
    public const string YesMentalHealthMedication = "Yes";
    public const string NoMentalHealthMedication = "No";
    public const string NotSureMentalHealthMedication = "Not Sure";
}