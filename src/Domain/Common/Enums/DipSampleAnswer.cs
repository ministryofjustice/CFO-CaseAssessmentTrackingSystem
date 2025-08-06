using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class DipSampleAnswer : SmartEnum<DipSampleAnswer>
{
    public bool IsAnswer { get; }
    
    public static readonly DipSampleAnswer NotAnswered = new("Not Answered", 0, isAnswer: false);
    public static readonly DipSampleAnswer Yes = new("Yes", 1);
    public static readonly DipSampleAnswer No = new("No", 2);
    public static readonly DipSampleAnswer NotApplicable = new("Not Applicable", 3);

    private DipSampleAnswer(string name, int value, bool isAnswer = true) : base(name, value)
    {
        IsAnswer = isAnswer;
    }
}