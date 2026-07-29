using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class UserStatus : SmartEnum<UserStatus>
{
    public static readonly UserStatus Inactive = new(nameof(Inactive), 0, "Inactive");
    public static readonly UserStatus Active = new(nameof(Active), 1, "Active");
    public static readonly UserStatus MarkAsInReview = new(nameof(MarkAsInReview), 2, "In-Review");
    public static readonly UserStatus Left = new(nameof(Left), 3, "Left");
    public static readonly UserStatus PendingActivation = new(nameof(PendingActivation), 4, "Pending Activation");

    private UserStatus(string name, int value, string displayName)
        : base(name, value)
    {
        DisplayName = displayName;
    }

    public string DisplayName { get; }

    public bool CanSignIn => this == Active || this == PendingActivation;
}
