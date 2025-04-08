using Cfo.Cats.Domain.Entities.Documents;

namespace Cfo.Cats.Application.Features.Activities.DTOs;

public class EducationTrainingDto
{
    public required string ParticipantId { get; set; }

    [Description("Course Title")]
    public string? CourseTitle { get; set; }

    [Description("Course hyperlink (if applicable)")]
    public string? CourseUrl { get; set; }

    [Description("Course Level")]
    public string? CourseLevel { get; set; }

    [Description("Course Commenced Date")]
    public DateTime? CourseCommencedOn { get; set; }

    [Description("Course Completed Date (if applicable)")]
    public DateTime? CourseCompletedOn { get; set; }

    [Description("Passed")]
    public CourseCompletionStatus? CourseCompletionStatus { get; set; }

    [Description("Document")]
    public Document? Document { get; set; }

    public class Validator : AbstractValidator<EducationTrainingDto>
    {
        public Validator()
        {
            RuleFor(c => c.CourseTitle)
                .MaximumLength(200)
                .NotNull()
                .WithMessage("You must enter a Course Title");

            RuleFor(c => c.CourseLevel)
                .MaximumLength(100)
                .NotNull()
                .WithMessage("You must choose a Course Level");

            RuleFor(c => c.CourseCommencedOn)
                .NotNull()
                .WithMessage("You must enter Course Commenced Date");

            RuleFor(course => course.CourseCompletedOn)
                        .GreaterThanOrEqualTo(course => course.CourseCommencedOn)
                        .When(course => course.CourseCompletedOn.HasValue && course.CourseCommencedOn.HasValue)
                        .WithMessage("Course completed date cannot be before the commenced date");

            RuleFor(v => v.CourseCompletionStatus)
                .NotNull()
                .WithMessage("You must choose a value for Passed");

            RuleFor(c => c.CourseUrl)                
                .MaximumLength(2000);
        }
    }
}