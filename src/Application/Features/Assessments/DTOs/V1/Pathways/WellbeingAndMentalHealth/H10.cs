namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;

public class H10() : SingleChoiceQuestion("How often do you feel stressed?",
    [
        EveryDay,
        OneTwoTimesAWeek,
        FewTimesAMonth,
        RarelyOrNever
    ])
{
    public const string EveryDay = "Every day";
    public const string OneTwoTimesAWeek = "1-2 times a week";
    public const string FewTimesAMonth = "Few times a month";
    public const string RarelyOrNever = "Rarely or never";
}

                    
