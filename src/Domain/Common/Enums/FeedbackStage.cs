using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public sealed class FeedbackStage : SmartEnum<FeedbackStage>
{
    public static readonly FeedbackStage SecondPass = new("Second Pass", 0);
    public static readonly FeedbackStage Escalation = new("Escalation", 1);
   
    private FeedbackStage(string name, int value)
        : base(name, value)
    {
    }
}