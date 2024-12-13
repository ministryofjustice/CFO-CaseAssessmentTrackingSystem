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

            RuleFor(v => v.Document)
                    .NotNull()
                    .WithMessage("You must upload a Education/Training Template")
                    .Must(file => NotExceedMaximumFileSize(file, Infrastructure.Constants.Documents.ActivityTemplate.MaximumSizeInMegabytes))
                    .WithMessage($"File size exceeds the maxmimum allowed size of {Infrastructure.Constants.Documents.ActivityTemplate.MaximumSizeInMegabytes} megabytes")
                    .MustAsync(BePdfFile)
                    .WithMessage("File is not a PDF");

            RuleFor(v => v.CourseCompletionStatus)
                .NotNull()
                .WithMessage("You must choose a value for Passed");
        }

        private static bool NotExceedMaximumFileSize(IBrowserFile? file, double maxSizeMB)
                    => file?.Size < ByteSize.FromMegabytes(maxSizeMB).Bytes;

        private async Task<bool> BePdfFile(IBrowserFile? file, CancellationToken cancellationToken)
        {
            if (file is null)
                return false;

            // Check file extension
            if (!Path.GetExtension(file.Name).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                return false;

            // Check MIME type
            if (file.ContentType != "application/pdf")
                return false;

            long maxSizeBytes = Convert.ToInt64(ByteSize.FromMegabytes(Infrastructure.Constants.Documents.ActivityTemplate.MaximumSizeInMegabytes).Bytes);

            // Check file signature (magic numbers)
            using (var stream = file.OpenReadStream(maxSizeBytes, cancellationToken))
            {
                byte[] buffer = new byte[4];
                await stream.ReadExactlyAsync(buffer.AsMemory(0, 4), cancellationToken);
                string header = System.Text.Encoding.ASCII.GetString(buffer);
                return header == "%PDF";
            }
        }
    }
}
