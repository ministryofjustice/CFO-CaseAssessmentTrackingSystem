namespace Cfo.Cats.Application.Common.Interfaces.Identity;

public interface IIdentitySettings
{
    bool RequireDigit { get; }
    int RequiredLength { get; }
    int MaxLength { get; }
    bool RequireNonAlphanumeric { get; }
    bool RequireUpperCase { get; }
    bool RequireLowerCase { get; }
    int DefaultLockoutTimeSpan { get; }
}
