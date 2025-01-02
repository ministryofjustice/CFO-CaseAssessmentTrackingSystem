using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class CourseCompletionStatus(string name, int value) : SmartEnum<CourseCompletionStatus>(name, value)
{
    public static readonly CourseCompletionStatus NotApplicable = new("Not Applicable", -1);
    public static readonly CourseCompletionStatus No = new(nameof(No), 0);
    public static readonly CourseCompletionStatus Yes = new(nameof(Yes), 1);
}
