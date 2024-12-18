using Cfo.Cats.Application.Common.Validators;
using Humanizer.Bytes;
using Microsoft.AspNetCore.Components.Forms;

namespace Cfo.Cats.Application.Features.Payables.DTOs;

public class EmploymentDto
{
    [Description("Employment Type")]
    public string? EmploymentType { get; set; }

    [Description("Company Name")]
    public string? EmployerName { get; set; }

    [Description("Job title")]
    public string? JobTitle { get; set; }

    [Description("Job Title Code")] 
    public string? JobTitleCode { get; set; }

    [Description("Salary")]
    public double? Salary { get; set; }

    [Description("Salary Frequency")]
    public string? SalaryFrequency { get; set; }

    [Description("Start date of employment")]
    public DateTime? EmploymentCommenced { get; set; }

    [Description("Upload Employment Template")]
    public IBrowserFile? Document { get; set; }

    public class Validator : AbstractValidator<EmploymentDto>
    {
        public Validator()
        {
            RuleFor(c => c.EmploymentType)
                .NotNull()
                .WithMessage("You must choose a Employment Type");

            RuleFor(c => c.EmployerName)
                .NotNull()
                .WithMessage("You must enter Company Name");

            RuleFor(c => c.JobTitle)
                .NotNull()
                .WithMessage("You must choose a valid Job title");

            When(c => c.JobTitle is not null, () =>
            {
                RuleFor(c => c.JobTitleCode)
                    .NotNull()
                    .WithMessage("You must choose a valid Job title");
            });

            When(c => c.Salary is not null, () =>
            {
                RuleFor(c => c.Salary!.Value)
                .Must(BeValidSalary)
                .WithMessage("You must enter a valid amount for salary i.e. a number with a Maximum of 2 digits after decimal point.");

                RuleFor(c => c.SalaryFrequency)
                .NotNull()
                .WithMessage("You must choose a Salary frequency");

            });

            RuleFor(c => c.EmploymentCommenced)
                .NotNull()
                .WithMessage("You must choose a start date of employment");

            RuleFor(v => v.Document)
                    .NotNull()
                    .WithMessage("You must upload a Employment Template")
                    .Must(file => NotExceedMaximumFileSize(file, Infrastructure.Constants.Documents.ActivityTemplate.MaximumSizeInMegabytes))
                    .WithMessage($"File size exceeds the maxmimum allowed size of {Infrastructure.Constants.Documents.ActivityTemplate.MaximumSizeInMegabytes} megabytes")
                    .MustAsync(BePdfFile)
                    .WithMessage("File is not a PDF");

        }
        private bool BeValidSalary(double salary)
        {

                // Convert the number to its fractional part
                var fractionalPart = salary - Math.Truncate(salary);

                // Check if the fractional part is not .25, .5, or .75
                return fractionalPart.ToString().Length <= 2;
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
