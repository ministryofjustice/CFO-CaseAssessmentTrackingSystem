namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.WellbeingAndMentalHealth;

public class H12() : SingleChoiceQuestion("How often do you feel stressed?", [
    EveryDay,
    OneToTwoTimesPerWeek,
    FewTimesAMonth,
    RarelyOrNever
])
{
    public const string EveryDay = "Every day";
    public const string OneToTwoTimesPerWeek = "1-2 times a week";
    public const string FewTimesAMonth = "Few times a month";
    public const string RarelyOrNever = "Rarely or never";

}
