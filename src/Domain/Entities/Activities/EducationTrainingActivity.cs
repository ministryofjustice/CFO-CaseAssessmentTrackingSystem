using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.Activities;

public class EducationTrainingActivity : ActivityWithTemplate
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    EducationTrainingActivity()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    EducationTrainingActivity(
        ActivityContext context, 
        string courseTitle, 
        string? courseUrl, 
        string courseLevel,
        DateTime courseCommencedOn, 
        DateTime? courseCompletedOn,
        CourseCompletionStatus courseCompletionStatus) : base(context) 
    {
        CourseTitle = courseTitle;
        CourseUrl = courseUrl;
        CourseLevel = courseLevel;
        CourseCommencedOn = courseCommencedOn;
        CourseCompletedOn = courseCompletedOn;
        CourseCompletionStatus = courseCompletionStatus;
    }

    public string CourseTitle { get; private set; }
    public string? CourseUrl { get; private set; }
    public string CourseLevel { get; private set; }
    public DateTime CourseCommencedOn { get; private set; }
    public DateTime? CourseCompletedOn { get; private set; }
    public CourseCompletionStatus CourseCompletionStatus { get; private set; }

    public override string DocumentLocation => "activity/educationandtraining";

    public static EducationTrainingActivity Create(
        ActivityContext context,
        string courseTitle,
        string? courseUrl,
        string courseLevel,
        DateTime courseCommencedOn,
        DateTime? courseCompletedOn,
        CourseCompletionStatus courseCompletionStatus)
    {
        EducationTrainingActivity activity = new(context, courseTitle, courseUrl, courseLevel, courseCommencedOn, courseCompletedOn, courseCompletionStatus);
        activity.AddDomainEvent(new EducationTrainingActivityCreatedDomainEvent(activity));
        return activity;
    }

}
