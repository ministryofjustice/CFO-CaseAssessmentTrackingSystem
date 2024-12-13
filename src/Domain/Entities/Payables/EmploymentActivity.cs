using Cfo.Cats.Domain.Entities.Documents;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.Payables;

public class EmploymentActivity : ActivityWithTemplate
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    EmploymentActivity()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    EmploymentActivity(
        ActivityContext context,
        string employmentType,
        string employerName,
        string jobTitle,
        string jobTitleCode,
        double? salary,
        string? salaryFrequency,
        DateTime employmentCommenced) : base(context) 
    {
        EmploymentType = employmentType;
        EmployerName = employerName;
        JobTitle = jobTitle;
        JobTitleCode = jobTitleCode;
        Salary = salary;
        SalaryFrequency = salaryFrequency;
        EmploymentCommenced = employmentCommenced;
    }

    public string EmploymentType { get; private set; }
    public string EmployerName { get; private set; }
    public string JobTitle { get; private set; }
    public string JobTitleCode { get; private set; } // SOC Code
    public double? Salary { get; private set; }
    public string? SalaryFrequency { get; private set; }
    public DateTime EmploymentCommenced { get; private set; }

    public override string DocumentLocation => "activity/employment";

    public static EmploymentActivity Create(
        ActivityContext context,
        string employmentType,
        string employerName,
        string jobTitle,
        string jobTitleCode,
        double? salary,
        string? salaryFrequency,
        DateTime employmentCommenced)
    {
        EmploymentActivity activity = new(context, employmentType, employerName, jobTitle, jobTitleCode, salary, salaryFrequency, employmentCommenced);
        activity.AddDomainEvent(new EmploymentActivityCreatedDomainEvent(activity));
        return activity;
    }
}
