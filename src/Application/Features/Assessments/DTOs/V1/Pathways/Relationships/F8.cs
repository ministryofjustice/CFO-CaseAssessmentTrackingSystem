namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;

public class F8() : SingleChoiceQuestion("Are you a carer?", 
    "For example, do you provide unpaid care for family or a friend who needs help due to their illness, disability, mental health problem or an addiction? This can be officially or unofficially.",
    [
        No,
        UnderTenHrsPerWeek,
        Between10And34HrsPerWeek,
        Between35And49HrsPerWeek,
        FiftyPlusHrsPerWeek
    ])
{
    public const string No = "No";
    public const string UnderTenHrsPerWeek = "Yes - Under 10 hrs per week";
    public const string Between10And34HrsPerWeek = "Yes - 10 - 34 hrs per week";
    public const string Between35And49HrsPerWeek = "Yes - 35 - 49 hrs per week";
    public const string FiftyPlusHrsPerWeek = "Yes - 50+ hrs per week";
}