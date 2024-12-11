using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Payables.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Humanizer.Bytes;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cfo.Cats.Application.Features.Payables.Commands;

public static class AddIsw
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result<bool>>
    {
        [Description("Wraparound Support Start Date")]
        public DateTime? WraparoundSupportStartDate { get; set; }

        [Description("Baseline Achieved Date")]
        public DateTime? BaselineAchievedDate { get; set; }

        [Description("Total Hours Performed Pre-intervention")]
        public decimal TotalHoursPerformedPreIntervention { get; set; } = 0;

        [Description("Total Hours Performed During Intervention")]
        public decimal TotalHoursPerformedDuringIntervention { get; set; } =0;

        [Description("Total Hours Performed After Intervention")]
        public decimal TotalHoursPerformedAfterIntervention { get; set; } = 0;

        [Description("Total Hours pre, during and after intervention")]
        public decimal TotalHoursIntervention { 
            get {
                return TotalHoursPerformedPreIntervention + TotalHoursPerformedDuringIntervention + TotalHoursPerformedAfterIntervention;
            } 
        }

        string TotalHoursDescription => string.Concat((int)Math.Floor(TotalHoursIntervention), " hour", Math.Floor(TotalHoursIntervention) is 1 ? string.Empty : "s");
        string TotalMinsDescription => string.Concat((int)(TotalHoursIntervention % 1 * 60), " mins");

        public string TotalHoursInterventionDescription => $"{TotalHoursDescription} {TotalMinsDescription}";

        [Description("Upload ISW Template")]
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

            RuleFor(c => c.WraparoundSupportStartDate)
                .NotNull()
                .WithMessage("You must enter Wraparound support start date");

            RuleFor(c => c.BaselineAchievedDate)
                .NotNull()
                .WithMessage("You must enter Baseline achieved date");

            RuleFor(x => x.TotalHoursPerformedPreIntervention)
                .Must(BeValidNumber)
                .WithMessage("Please enter a valid number, digits after decimal point may only contain 0, .25 or .5 or .75");
            RuleFor(x => x.TotalHoursPerformedDuringIntervention)
                .Must(BeValidNumber)
                .WithMessage("Please enter a valid number, digits after decimal point may only contain 0, .25 or .5 or .75");
            RuleFor(x => x.TotalHoursPerformedAfterIntervention)
                .Must(BeValidNumber)
                .WithMessage("Please enter a valid number, digits after decimal point may only contain 0, .25 or .5 or .75");

            RuleFor(c => (c.TotalHoursPerformedPreIntervention + c.TotalHoursPerformedDuringIntervention + c.TotalHoursPerformedAfterIntervention))
                .GreaterThanOrEqualTo(10)
                .WithMessage("Total Intervention Hours (pre, during and after) must be atleast 10 hours or more");

            

            RuleFor(v => v.Document)
                    .NotNull()
                    .WithMessage("You must upload a Right to Work document")
                    .Must(file => NotExceedMaximumFileSize(file, Infrastructure.Constants.Documents.RightToWork.MaximumSizeInMegabytes))
                    .WithMessage($"File size exceeds the maxmimum allowed size of {Infrastructure.Constants.Documents.RightToWork.MaximumSizeInMegabytes} megabytes")
                    .MustAsync(BePdfFile)
                    .WithMessage("File is not a PDF");

            
        }
        private bool BeValidNumber(decimal number)
        { 
            // Convert the number to its fractional part
            var fractionalPart = number - Math.Truncate(number); 
            
            // Check if the fractional part is not .25, .5, or .75
            return fractionalPart == 0.0m || fractionalPart == 0.25m || fractionalPart == 0.5m || fractionalPart == 0.75m;
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