using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Humanizer.Bytes;
using Microsoft.AspNetCore.Components.Forms;
namespace Cfo.Cats.Application.Features.Payables.Commands;

public static class AddEmployment
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result<bool>>
    {
        [Description("Employment Type")]
        public string? EmploymentType { get; set; }

        [Description("Company Name")]
        public string? CompanyName { get; set; }

        [Description("Job Title")]
        public string? JobTitle { get; set; }

        [Description("Salary")]
        public string? Salary { get; set; }

        [Description("Salary Frequency")]
        public string? SalaryFrequency { get; set; }

        [Description("Employment Start Date")]
        public DateTime? EmploymentStartDate { get; set; }

        [Description("Upload Employment Template")]
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

            RuleFor(c => c.EmploymentType)
                .NotNull()
                .WithMessage("You must choose a Employment Type");

            RuleFor(c => c.CompanyName)
                .NotNull()
                .MaximumLength(100);
            
            RuleFor(c => c.JobTitle)
                .NotNull()
                .WithMessage("You must choose a Job title");

            RuleFor(c => c.Salary)
                .Matches(ValidationConstants.NumberWithTwoDecimalPlaces)
                .WithMessage("You must enter a valid amount for salary i.e. number with a Maximum of 2 digits after decimal point.");

            RuleFor(c => c.SalaryFrequency)
                .NotNull()
                .WithMessage("You must choose a Salary frequency");

            RuleFor(c => c.EmploymentStartDate)
                .NotNull()
                .WithMessage("You must choose a Employment start date");

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