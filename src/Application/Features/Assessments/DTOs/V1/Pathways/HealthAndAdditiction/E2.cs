namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.HealthAndAdditiction;

public class E2(): SingleChoiceQuestion("Do you consider yourself disabled?", "For example, do you have a long-term physical or mental health condition/illness that reduces your ability to carry-out day-to-day activities?",
    [
        NotDisabled,
        LittleImpairment,
        LotOfImpairment
        
    ])
{
    public const string NotDisabled = "No";
    public const string LittleImpairment = "Yes, A little impairment";
    public const string LotOfImpairment = "Yes, A lot of impairment";
}