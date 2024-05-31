using System.ComponentModel;

namespace Cfo.Cats.Infrastructure.PermissionSet;

public static partial class Permissions
{
    [DisplayName("Enrolments")]
    public static class Enrolments
    {
        public const string Search = "Permissions.Enrolments.Search";
    }
}