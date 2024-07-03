namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Housing;

public class B6() : MultipleChoiceQuestion(
    "Do you live with any of the following? (tick all that apply)",
    "If you’re in prison, then think about who you expect to live with on release.",
    [
        LivingWithPartnerOrSpouse,
        LivingWithOtherFamilyMembers,
        LivingWithOwnChildren,
        LivingWithOtherNonFamilyMembers,
        LivingWithOtherChildren,
        LiveAlone
    ])
{
    public const string LivingWithPartnerOrSpouse = "Partner/spouse";
    public const string LivingWithOtherFamilyMembers = "Other family members";
    public const string LivingWithOwnChildren = "Own Children";
    public const string LivingWithOtherNonFamilyMembers = "Other non-family members";
    public const string LivingWithOtherChildren = "Other Children";
    public const string LiveAlone = "Live alone";
}