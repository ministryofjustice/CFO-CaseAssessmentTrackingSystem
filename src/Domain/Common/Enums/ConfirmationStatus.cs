using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class ConfirmationStatus : SmartEnum<ConfirmationStatus>
{
    public static readonly ConfirmationStatus Unknown = new(nameof(Unknown), -1);
    public static readonly ConfirmationStatus No = new(nameof(No), 0);
    public static readonly ConfirmationStatus Yes = new(nameof(Yes), 1);

    private ConfirmationStatus(string name, int value)
        : base(name, value) { }
}
