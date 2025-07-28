using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class ComplianceAnswer : SmartEnum<ComplianceAnswer>
{
    public bool IsAnswer { get; }
    public bool IsAccepted { get; }
    public bool IsAuto { get; }

    public static readonly ComplianceAnswer NotAnswered = new("Not Answered", 0, isAnswer: false);
    public static readonly ComplianceAnswer Compliant = new("Compliant", 1, isAccepted: true);
    public static readonly ComplianceAnswer NotCompliant = new("Not Compliant", 2, isAccepted: false);
    public static readonly ComplianceAnswer AutoCompliant = new("Compliant (Auto)", 3, isAccepted: true, isAuto: true);
    public static readonly ComplianceAnswer AutoNotCompliant = new("Not Compliant (Auto)", 4, isAccepted: false, isAuto: true);
    public static readonly ComplianceAnswer Unsure = new("Unsure", 5, isAccepted: false);

    private ComplianceAnswer(string name, int value, bool isAnswer = true, bool isAccepted = false, bool isAuto = false) : base(name, value)
    {
        IsAnswer = isAnswer;
        IsAccepted = isAccepted;
        IsAuto = isAuto;
    }
}