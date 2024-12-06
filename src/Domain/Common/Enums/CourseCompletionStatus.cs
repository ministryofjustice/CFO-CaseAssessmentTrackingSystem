using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class CourseCompletionStatus(string name, int value) : SmartEnum<CourseCompletionStatus>(name, value)
{
    public static readonly CourseCompletionStatus NotApplicable = new(nameof(NotApplicable), -1);
    public static readonly CourseCompletionStatus Failed = new(nameof(Failed), 0);
    public static readonly CourseCompletionStatus Passed = new(nameof(Passed), 1);
}
