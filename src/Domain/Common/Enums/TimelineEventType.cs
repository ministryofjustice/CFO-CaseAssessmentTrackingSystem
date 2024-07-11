using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class TimelineEventType : SmartEnum<TimelineEventType>
{

    public static readonly TimelineEventType Participant = new(nameof(Participant), 0);
    public static readonly TimelineEventType Enrolment = new(nameof(Enrolment), 1);
    public static readonly TimelineEventType Consent = new(nameof(Consent), 2);
    public static readonly TimelineEventType Assessment = new(nameof(Participant), 3);
    
    private TimelineEventType(string name, int value) : base(name, value)
    {
    }
}
