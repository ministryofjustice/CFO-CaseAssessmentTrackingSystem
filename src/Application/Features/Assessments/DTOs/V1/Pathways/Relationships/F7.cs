namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;

public class F7() : MultipleChoiceQuestion("Do you feel you can trust, confide in, and rely on any of the following? (tick all that apply)",
    [
        PartnerOrSpouse,
        CloseFriend,
        ParentOrGuardian,
        CaseOrSupportWorker,
        OtherFamilyMember,
        NoneOfThese,
    ])
{
    public const string PartnerOrSpouse = "A partner / spouse";
    public const string CloseFriend = "A close friend";
    public const string ParentOrGuardian = "A parent / guardian";
    public const string CaseOrSupportWorker = "A case/support worker";
    public const string OtherFamilyMember = "Other family member";
    public const string NoneOfThese = "None of these";
}