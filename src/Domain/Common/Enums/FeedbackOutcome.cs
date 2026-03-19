using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public sealed class FeedbackOutcome : SmartEnum<FeedbackOutcome>
{
    public static readonly FeedbackOutcome Approved = new("Approved", 0);
    public static readonly FeedbackOutcome Returned = new("Returned", 1);
    public static readonly FeedbackOutcome Escalated = new("Escalated", 2);
    public static readonly FeedbackOutcome EscalatedComment = new(" Comment", 3);
    
    private FeedbackOutcome(string name, int value)
        : base(name, value)
    {
    }
}