namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.HealthAndAdditiction;

public class E5() : SingleChoiceQuestion("How much sport, exercise or physical activity do you do in a typical week?",
    "This can be anything that raises your heart-rate or gets you out of breath.",
    [
        LessThan30MinutesPerWeek,
        ThirtyMinutesTo2AndHalfHoursPerWeek,
        Over2AndHalfHoursPerWeek
    ])
{
    public const string LessThan30MinutesPerWeek = "Less than 30min per week";
    public const string ThirtyMinutesTo2AndHalfHoursPerWeek = "30min to 2Â½ hr per week";
    public const string Over2AndHalfHoursPerWeek = "Over 2Â½ hrs per week";
    
}