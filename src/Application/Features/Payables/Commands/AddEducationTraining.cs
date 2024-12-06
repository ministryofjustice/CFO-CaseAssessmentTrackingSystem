using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Humanizer.Bytes;
using Microsoft.AspNetCore.Components.Forms;

namespace Cfo.Cats.Application.Features.Payables.Commands;

public static class AddEducationTraining
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result<bool>>
    {
        [Description("Course Title")]
        public string? CourseTitle { get; set; }

        [Description("Course Hyperlink")]
        public string? CourseHyperlink { get; set; }

        [Description("Course Level")]
        public string? CourseLevel { get; set; }

        [Description("Course Commenced Date")]
        public DateTime? CourseCommencedDate { get; set; }

        [Description("Course Completed Date")]
        public DateTime? CourseCompletedDate { get; set; }

        [Description("Passed")]
        public string? Passed { get; set; }

        [Description("Upload Education/Training Template")]
        public IBrowserFile? Document { get; set; }
    }

    class Handler: IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            // TODO: record activity
            return await Task.FromResult(true);
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        readonly IUnitOfWork unitOfWork;
        public Validator(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;

            RuleFor(c => c.CourseTitle)
                .NotNull()
                .MaximumLength(100)
                .WithMessage("You must enter a Course Title");
            
            RuleFor(c => c.CourseLevel)
                .NotNull()
                .WithMessage("You must choose a Course Level");

            RuleFor(c => c.CourseCommencedDate)
                .NotNull()
                .WithMessage("You must enter Course Commenced Date"); 
            
            RuleFor(c => c.Passed)
                .NotNull()
                .WithMessage("You must choose a value for Passed");

            RuleFor(v => v.Document)
                    .NotNull()
                    .WithMessage("You must upload a Right to Work document")
                    .Must(file => NotExceedMaximumFileSize(file, Infrastructure.Constants.Documents.RightToWork.MaximumSizeInMegabytes))
                    .WithMessage($"File size exceeds the maxmimum allowed size of {Infrastructure.Constants.Documents.RightToWork.MaximumSizeInMegabytes} megabytes")
                    .MustAsync(BePdfFile)
                    .WithMessage("File is not a PDF");
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

            long maxSizeBytes = Convert.ToInt64(ByteSize.FromMegabytes(Infrastructure.Constants.Documents.RightToWork.MaximumSizeInMegabytes).Bytes);

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