using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class TimelineEventType : SmartEnum<TimelineEventType>
{

    public static readonly TimelineEventType Participant = new(nameof(Participant), 0);
    public static readonly TimelineEventType Enrolment = new(nameof(Enrolment), 1);
    public static readonly TimelineEventType Consent = new(nameof(Consent), 2);
    public static readonly TimelineEventType Assessment = new(nameof(Assessment), 3);
    public static readonly TimelineEventType PathwayPlan = new(nameof(PathwayPlan), 4);
    public static readonly TimelineEventType Bio = new(nameof(Bio), 5);
    public static readonly TimelineEventType Dms = new(nameof(Dms), 6);
    public static readonly TimelineEventType PRI = new(nameof(PRI), 7);
    public static readonly TimelineEventType Activity = new(nameof(Activity), 8);

    private TimelineEventType(string name, int value) : base(name, value)
    {
    }
}