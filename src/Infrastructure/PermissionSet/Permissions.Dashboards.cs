using System.ComponentModel;

namespace Cfo.Cats.Infrastructure.PermissionSet;

public static partial class Permissions
{
    [DisplayName("Dashboard")]
    [Description("Dashboard Permissions")]
    public static class Dashboards
    {
        public const string View = "Permissions.Dashboards.View";
    }
}
