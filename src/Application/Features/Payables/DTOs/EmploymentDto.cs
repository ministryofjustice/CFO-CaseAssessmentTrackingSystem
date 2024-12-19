using Cfo.Cats.Domain.Entities.Documents;

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

    [Description("Document")]
    public Document? Document { get; set; }

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
        }
        private bool BeValidSalary(double salary)
        {

                // Convert the number to its fractional part
                var fractionalPart = salary - Math.Truncate(salary);

                // Check if the fractional part is not .25, .5, or .75
                return fractionalPart.ToString().Length <= 2;
        }
    }
}
