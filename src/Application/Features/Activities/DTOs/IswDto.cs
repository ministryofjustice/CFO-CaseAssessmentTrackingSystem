﻿using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Domain.Entities.Documents;

namespace Cfo.Cats.Application.Features.Activities.DTOs;

public class IswDto
{
    [Description("Wraparound support start date")]
    public DateTime? WraparoundSupportStartedOn { get; set; }

    [Description("Total hours performed pre-intervention")]
    public double HoursPerformedPre { get; set; } = 0;

    [Description("Total hours performed during intervention")]
    public double HoursPerformedDuring { get; set; } = 0;

    [Description("Total hours performed post intervention")]
    public double HoursPerformedPost { get; set; } = 0;

    [Description("Baseline achieved date")]
    public DateTime? BaselineAchievedOn { get; set; }

    [Description("Total Hours pre, during, and after intervention")]
    public double TotalHoursIntervention => HoursPerformedPre + HoursPerformedDuring + HoursPerformedPost;

    string TotalHoursDescription => string.Concat((int)Math.Floor(TotalHoursIntervention), " hour", Math.Floor(TotalHoursIntervention) is 1 ? string.Empty : "s");
    string TotalMinsDescription => string.Concat((int)(TotalHoursIntervention % 1 * 60), " mins");
    public string TotalHoursInterventionDescription => $"{TotalHoursDescription} {TotalMinsDescription}";


    [Description("Document")]
    public Document? Document { get; set; }

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
        }

        private bool BeValidNumber(double number)
        {
            // Convert the number to its fractional part
            var fractionalPart = number - Math.Truncate(number);

            // Check if the fractional part is not .25, .5, or .75
            return fractionalPart == 0.0 || fractionalPart == 0.25 || fractionalPart == 0.5 || fractionalPart == 0.75;
        }

    }

}