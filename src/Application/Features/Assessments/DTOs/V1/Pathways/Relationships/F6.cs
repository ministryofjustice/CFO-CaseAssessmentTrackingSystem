namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;

public class F6() : SingleChoiceQuestion("On a scale 1 to 5, in general how much do you trust most people?",
    [
        NotAtAll,
        ALittle,
        SomeWhat,
        Mostly,
        Completely,
    ])
{
    public const string NotAtAll = "1 Not at all";
    public const string ALittle = "2 A little";
    public const string SomeWhat = "3 Somewhat";
    public const string Mostly = "4 Mostly";
    public const string Completely = "5 Completely";
}