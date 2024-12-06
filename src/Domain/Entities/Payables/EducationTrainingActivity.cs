using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Documents;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.Payables;

public class EducationTrainingActivity : Activity
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
        CourseCompletionStatus courseCompletionStatus) : base(context) 
    {
        CourseTitle = courseTitle;
        CourseUrl = courseUrl;
        CourseLevel = courseLevel;
        CourseCommencedOn = courseCommencedOn;
        CourseCompletionStatus = courseCompletionStatus;
    }

    public string CourseTitle { get; private set; }
    public string? CourseUrl { get; private set; }
    public string CourseLevel { get; private set; }
    public DateTime CourseCommencedOn { get; private set; }
    public CourseCompletionStatus CourseCompletionStatus { get; private set; }
    public bool Passed { get; private set; }
    public virtual Document? Document { get; private set; } // Uploaded template

    public static EducationTrainingActivity Create(
        ActivityContext context,
        string courseTitle,
        string? courseUrl,
        string courseLevel,
        DateTime courseCommencedOn,
        CourseCompletionStatus courseCompletionStatus)
    {
        EducationTrainingActivity activity = new(context, courseTitle, courseUrl, courseLevel, courseCommencedOn, courseCompletionStatus);
        activity.AddDomainEvent(new EducationTrainingActivityCreatedDomainEvent(activity));
        return activity;
    }

    public EducationTrainingActivity AddDocument(Document document)
    {
        Document = document;
        return this;
    }

}
