namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Education;
public class D2() : SingleChoiceQuestion("Did you finish school?",
    [
        YesFinishedSchool,
        LeftBefore16,
        LeftBefore11
    ])
{ 
    public const string YesFinishedSchool = "Yes";
    public const string LeftBefore16 = "Left before age 16";
    public const string LeftBefore11 = "Left before age 11";
}