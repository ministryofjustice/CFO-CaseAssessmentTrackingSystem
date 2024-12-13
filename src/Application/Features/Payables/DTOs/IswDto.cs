using Cfo.Cats.Application.Common.Validators;
using Humanizer.Bytes;
using Microsoft.AspNetCore.Components.Forms;

namespace Cfo.Cats.Application.Features.Payables.DTOs;

public class IswDto
{
    [Description("Wraparound Support Start Date")]
    public DateTime? WraparoundSupportStartedOn { get; set; }

    [Description("Total Hours Performed Pre-intervention")]
    public double HoursPerformedPre { get; set; } = 0;

    [Description("Total Hours Performed During Intervention")]
    public double HoursPerformedDuring { get; set; } = 0;

    [Description("Total Hours Performed After Intervention")]
    public double HoursPerformedPost { get; set; } = 0;

    [Description("Baseline Achieved Date")]
    public DateTime? BaselineAchievedOn { get; set; }

    [Description("Upload ISW Template")]
    public IBrowserFile? Document { get; set; }

    [Description("Total Hours pre, during and after intervention")]
    public double TotalHoursIntervention => HoursPerformedPre + HoursPerformedDuring + HoursPerformedPost;

    string TotalHoursDescription => string.Concat((int)Math.Floor(TotalHoursIntervention), " hour", Math.Floor(TotalHoursIntervention) is 1 ? string.Empty : "s");
    string TotalMinsDescription => string.Concat((int)(TotalHoursIntervention % 1 * 60), " mins");
    public string TotalHoursInterventionDescription => $"{TotalHoursDescription} {TotalMinsDescription}";

    public class Validator : AbstractValidator<IswDto>
    {
        public Validator()
        {
            RuleFor(c => c.WraparoundSupportStartedOn)
                .NotNull()
                .WithMessage("You must enter Wraparound support start date")
                .LessThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage(ValidationConstants.DateMustBeInPast);

            RuleFor(c => c.BaselineAchievedOn)
                .NotNull()
                .WithMessage("You must enter Baseline achieved date")
                .LessThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage(ValidationConstants.DateMustBeInPast);

            RuleFor(x => x.HoursPerformedPre)
                .Must(BeValidNumber)
                .WithMessage("Please enter a valid number, digits after decimal point may only contain 0, .25 or .5 or .75");

            RuleFor(x => x.HoursPerformedDuring)
                .Must(BeValidNumber)
                .WithMessage("Please enter a valid number, digits after decimal point may only contain 0, .25 or .5 or .75");

            RuleFor(x => x.HoursPerformedPost)
                .Must(BeValidNumber)
                .WithMessage("Please enter a valid number, digits after decimal point may only contain 0, .25 or .5 or .75");

            RuleFor(c => c.TotalHoursIntervention)
                .GreaterThanOrEqualTo(10)
                .WithMessage("Total Intervention Hours (pre, during and after) must be atleast 10 hours or more");

            RuleFor(v => v.Document)
                    .NotNull()
                    .WithMessage("You must upload a ISW Template")
                    .Must(file => NotExceedMaximumFileSize(file, Infrastructure.Constants.Documents.ActivityTemplate.MaximumSizeInMegabytes))
                    .WithMessage($"File size exceeds the maxmimum allowed size of {Infrastructure.Constants.Documents.ActivityTemplate.MaximumSizeInMegabytes} megabytes")
                    .MustAsync(BePdfFile)
                    .WithMessage("File is not a PDF");
        }

        private bool BeValidNumber(double number)
        {
            // Convert the number to its fractional part
            var fractionalPart = number - Math.Truncate(number);

            // Check if the fractional part is not .25, .5, or .75
            return fractionalPart == 0.0 || fractionalPart == 0.25 || fractionalPart == 0.5 || fractionalPart == 0.75;
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