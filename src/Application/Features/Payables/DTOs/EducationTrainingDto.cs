using Humanizer.Bytes;
using Microsoft.AspNetCore.Components.Forms;

namespace Cfo.Cats.Application.Features.Payables.DTOs;

public class EducationTrainingDto
{
    [Description("Course Title")]
    public string? CourseTitle { get; set; }

    [Description("Course Url")]
    public string? CourseUrl { get; set; }

    [Description("Course Level")]
    public string? CourseLevel { get; set; }

    [Description("Course Commenced Date")]
    public DateTime? CourseCommencedOn { get; set; }

    [Description("Course Completed Date")]
    public DateTime? CourseCompletedDate { get; set; }

    [Description("Passed")]
    public CourseCompletionStatus? CourseCompletionStatus { get; set; }

    [Description("Upload Education/Training Template")]
    public IBrowserFile? Document { get; set; }

    public class Validator : AbstractValidator<EducationTrainingDto>
    {
        public Validator()
        {
            RuleFor(c => c.CourseTitle)
                .NotNull()
                .MaximumLength(100)
                .WithMessage("You must enter a Course Title");

            RuleFor(c => c.CourseLevel)
                .NotNull()
                .WithMessage("You must choose a Course Level");

            RuleFor(c => c.CourseCommencedOn)
                .NotNull()
                .WithMessage("You must enter Course Commenced Date");

            RuleFor(course => course.CourseCompletedDate)
                        .GreaterThanOrEqualTo(course => course.CourseCommencedOn)
                        .When(course => course.CourseCompletedDate.HasValue)
                        .WithMessage("Course completed date must be greater than Course commenced date");

            RuleFor(v => v.CourseCompletionStatus)
                .NotNull()
                .WithMessage("You must choose a value for Passed");
        }

    }
}
