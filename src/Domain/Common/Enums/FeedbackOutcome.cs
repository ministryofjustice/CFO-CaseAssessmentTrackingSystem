using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public sealed class FeedbackOutcome : SmartEnum<FeedbackOutcome>
{
    public static readonly FeedbackOutcome Approved = new("Approved", 0);
    public static readonly FeedbackOutcome Returned = new("Returned", 1);
    public static readonly FeedbackOutcome Escalated = new("Escalated", 2);
   
    private FeedbackOutcome(string name, int value)
        : base(name, value)
    {
    }
}