using Cfo.Cats.Domain.Entities.Documents;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.Payables;

public class ISWActivity : Activity
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    ISWActivity()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    ISWActivity(
        ActivityContext context,
        DateTime wraparoundSupportStartedOn,
        int hoursPerformedPre,
        int hoursPerformedDuring,
        int hoursPerformedPost,
        DateTime baselineAchievedOn) : base(context) 
    {
        WraparoundSupportStartedOn = wraparoundSupportStartedOn;
        HoursPerformedPre = hoursPerformedPre;
        HoursPerformedDuring = hoursPerformedDuring;
        HoursPerformedPost = hoursPerformedPost;
        BaselineAchievedOn = baselineAchievedOn;
    }

    public DateTime WraparoundSupportStartedOn { get; private set; }
    public int HoursPerformedPre { get; private set; }
    public int HoursPerformedDuring { get; private set; }
    public int HoursPerformedPost { get; private set; }
    public DateTime BaselineAchievedOn { get; private set; }
    public virtual Document? Document { get; private set; } // Uploaded template

    public static ISWActivity Create(
        ActivityContext context,
        DateTime wraparoundSupportStartedOn,
        int hoursPerformedPre,
        int hoursPerformedDuring,
        int hoursPerformedPost,
        DateTime baselineAchievedOn)
    {
        ISWActivity activity = new(context, wraparoundSupportStartedOn, hoursPerformedPre, hoursPerformedDuring, hoursPerformedPost, baselineAchievedOn);
        activity.AddDomainEvent(new ISWActivityCreatedDomainEvent(activity));
        return activity;
    }

    public ISWActivity AddDocument(Document document)
    {
        Document = document;
        return this;
    }

}