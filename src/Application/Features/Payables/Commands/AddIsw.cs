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
        public decimal? TotalHoursPerformedPreIntervention { get; set; }

        [Description("Total Hours Performed During Intervention")]
        public decimal? TotalHoursPerformedDuringIntervention { get; set; }

        [Description("Total Hours Performed After Intervention")]
        public decimal? TotalHoursPerformedAfterIntervention { get; set; }

        [Description("Additional Information")]
        public string? AdditionalInformation { get; set; }
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

            RuleFor(c => c.TotalHoursPerformedPreIntervention.ToString())
                .Matches(ValidationConstants.NumberWithTwoDecimalPlaces)
                .WithMessage("You must enter a valid value for Total hours performed pre-intervention i.e. number with a Maximum of 2 digits after decimal point for example 1.25 or 2.5 or 4.75 etc.");

            RuleFor(c => c.TotalHoursPerformedDuringIntervention.ToString())
                .Matches(ValidationConstants.NumberWithTwoDecimalPlaces)
                .WithMessage("You must enter a valid value for Total hours performed during intervention i.e. number with a Maximum of 2 digits after decimal point for example 1.25 or 2.5 or 4.75 etc.");

            RuleFor(c => c.TotalHoursPerformedAfterIntervention.ToString())
                .Matches(ValidationConstants.NumberBetweenZeroAndTenWithQuarterIncrement)
                .WithMessage("You must enter a valid value for Total hours performed after intervention i.e. number with a Maximum of 2 digits after decimal point for example 1.25,2.5 or 4.75 etc.");
        }

    }
}