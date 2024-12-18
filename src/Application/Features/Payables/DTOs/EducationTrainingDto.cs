using Humanizer.Bytes;
using Microsoft.AspNetCore.Components.Forms;

namespace Cfo.Cats.Application.Features.Payables.DTOs;

public class EducationTrainingDto
{
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

    public class Validator : AbstractValidator<EducationTrainingDto>
    {
        public Validator()
        {
            RuleFor(c => c.CourseTitle)
                .NotNull()
                .WithMessage("You must enter a Course Title");

            RuleFor(c => c.CourseLevel)
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
        }

    }
}
