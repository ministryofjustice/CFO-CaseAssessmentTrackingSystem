using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.Activities;

public class ISWActivity : ActivityWithTemplate
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ISWActivity()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private ISWActivity(
        ActivityContext context,
        DateTime wraparoundSupportStartedOn,
        double hoursPerformedPre,
        double hoursPerformedDuring,
        double hoursPerformedPost,
        DateTime baselineAchievedOn) : base(context) 
    {
        WraparoundSupportStartedOn = wraparoundSupportStartedOn;
        HoursPerformedPre = hoursPerformedPre;
        HoursPerformedDuring = hoursPerformedDuring;
        HoursPerformedPost = hoursPerformedPost;
        BaselineAchievedOn = baselineAchievedOn;
    }

    public DateTime WraparoundSupportStartedOn { get; private set; }
    public double HoursPerformedPre { get; private set; }
    public double HoursPerformedDuring { get; private set; }
    public double HoursPerformedPost { get; private set; }
    public DateTime BaselineAchievedOn { get; private set; }

    public override DateTime Expiry => BaselineAchievedOn.AddMonths(3);

    public override string DocumentLocation => "activity/isw";

    public static ISWActivity Create(
        ActivityContext context,
        DateTime wraparoundSupportStartedOn,
        double hoursPerformedPre,
        double hoursPerformedDuring,
        double hoursPerformedPost,
        DateTime baselineAchievedOn)
    {
        ISWActivity activity = new(context, wraparoundSupportStartedOn, hoursPerformedPre, hoursPerformedDuring, hoursPerformedPost, baselineAchievedOn);
        activity.AddDomainEvent(new ISWActivityCreatedDomainEvent(activity));
        return activity;
    }
}