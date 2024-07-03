namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Housing;

public class B3()
    : MultipleChoiceQuestion("Do you struggle with any of the following when at home? (tick all that apply)",
        [
            CookingOrMealPlanningOrHealthyEatingStruggle,
            CleaningOrLookingAfterYourHomeStruggle,
            KeepingYourHomeWarmStruggle,
            DampMouldOrCondensationStruggle,
            LackingEssentialFurnitureOrAppliancesStruggle,
            LackOfPrivacyOrYourOwnSpaceStruggle,
            ArrangingUtilitiesStruggle,
            NoneOftheGivenOptions
        ])
{
    public const string CookingOrMealPlanningOrHealthyEatingStruggle = "Cooking/meal planning/healthy eating";
    public const string CleaningOrLookingAfterYourHomeStruggle = "Cleaning or looking after your home";
    public const string KeepingYourHomeWarmStruggle = "Keeping your home warm";
    public const string DampMouldOrCondensationStruggle = "Damp, mould, or condensation";
    public const string LackingEssentialFurnitureOrAppliancesStruggle = "Lacking essential furniture or appliances";
    public const string LackOfPrivacyOrYourOwnSpaceStruggle = "Lack of privacy or your own space";
    public const string ArrangingUtilitiesStruggle = "Arranging utilities (gas/electric/phone/broadband etc.)";
    public const string NoneOftheGivenOptions = "None of the above";
}