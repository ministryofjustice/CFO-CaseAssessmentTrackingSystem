namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Education;

public class D5() : MultipleChoiceQuestion("In the last 12 months have you participated in any of the following creative activities? (tick all that apply)",
    [
        Painting,
        MakingFilms,
        Drawing,
        Photography,
        Crafts,
        PerformingMusic,
        WritingStoriesOrPoetry,
        PerformingDrama,
        Reading,
        Puzzles,
        OtherCreativeActivity,
        NoneOftheGivenOptions
    ])
{
    public const string Painting = "Painting";
    public const string MakingFilms = "Making films";
    public const string Drawing = "Drawing";
    public const string Photography = "Photography";
    public const string Crafts = "Crafts";
    public const string PerformingMusic = "Performing music";
    public const string WritingStoriesOrPoetry = "Writing stories/poetry";
    public const string PerformingDrama = "Performing drama";
    public const string Reading = "Reading";
    public const string Puzzles = "Puzzles";
    public const string OtherCreativeActivity = "Other creative activity";
    public const string NoneOftheGivenOptions = "None of these";
}
