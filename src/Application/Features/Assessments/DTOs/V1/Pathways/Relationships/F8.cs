namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;

public class F8() : SingleChoiceQuestion("Are you a carer?", 
    "For example, do you provide unpaid care for family or a friend who needs help due to their illness, disability, mental health problem or an addiction? This can be officially or unofficially.",
    [
        No,
        UnderTenHoursPerWeek,
        Between10And34HoursPerWeek,
        Between35And49HoursPerWeek,
        FiftyPlusHoursPerWeek
    ])
{
    public const string No = "No";
    public const string UnderTenHoursPerWeek = "Yes - Under 10 hrs per week";
    public const string Between10And34HoursPerWeek = "Yes - 10 - 34 hrs per week";
    public const string Between35And49HoursPerWeek = "Yes - 35 - 49 hrs per week";
    public const string FiftyPlusHoursPerWeek = "Yes - 50+ hrs per week";
}