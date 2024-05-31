using System.ComponentModel;

namespace Cfo.Cats.Infrastructure.PermissionSet;

public static partial class Permissions
{
    [DisplayName("Multi-Tenant")]
    [Description("Multi-Tenant Permissions")]
    public static class Tenants
    {
        public const string View = "Permissions.Tenants.View";
        public const string Create = "Permissions.Tenants.Create";
        public const string Edit = "Permissions.Tenants.Edit";
        public const string Delete = "Permissions.Tenants.Delete";
        public const string Search = "Permissions.Tenants.Search";
        public const string Export = "Permissions.Tenants.Export";
    }
}