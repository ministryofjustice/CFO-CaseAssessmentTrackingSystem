namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Housing;

public class B1() : SingleChoiceQuestion("Where do you normally live or spend most of your time?",
    "If you’re in prison, then think about where you expect to live on release. Try to choose the one option that best fits your situation.",
    [
        SleepRough,
        ShelterHostelEmergencyHousingOrAP,
        TemporarilyStayingWithFamilyOrFriends,
        TemporaryOrSupportedHousing,
        HousingRentedOrOwnedByYouOrYourPartnerParentOrGuardian
    ]
)
{
    public const string SleepRough = "Sleep rough";
    public const string ShelterHostelEmergencyHousingOrAP = "Shelter, hostel, emergency housing or AP";
    public const string TemporarilyStayingWithFamilyOrFriends = "Temporarily staying with family or friends";
    public const string TemporaryOrSupportedHousing = "Temporary or supported housing";
    public const string HousingRentedOrOwnedByYouOrYourPartnerParentOrGuardian = "Housing is rented or owned by you, your partner, parent or guardian";
}
