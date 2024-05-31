using System.ComponentModel;

namespace Cfo.Cats.Infrastructure.PermissionSet;

public static partial class Permissions
{
    [DisplayName("Audit Trails")]
    [Description("Audit Trails Permissions")]
    public static class AuditTrails
    {
        public const string View = "Permissions.AuditTrails.View";
        public const string Search = "Permissions.AuditTrails.Search";
        public const string Export = "Permissions.AuditTrails.Export";
    }
}
